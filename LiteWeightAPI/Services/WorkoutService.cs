using AutoMapper;
using LiteWeightAPI.Api.Common.Responses;
using LiteWeightAPI.Api.Users.Responses;
using LiteWeightAPI.Api.Workouts.Requests;
using LiteWeightAPI.Api.Workouts.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Services.Helpers;
using LiteWeightAPI.Services.Validation;
using NodaTime;

namespace LiteWeightAPI.Services;

public interface IWorkoutService
{
	Task<UserAndWorkoutResponse> CreateWorkout(CreateWorkoutRequest request, string userId);
	Task<UserAndWorkoutResponse> SwitchWorkout(SwitchWorkoutRequest request, string userId);
	Task<UserAndWorkoutResponse> CopyWorkout(CopyWorkoutRequest request, string userId);
	Task<UserAndWorkoutResponse> SetRoutine(SetRoutineRequest request, string workoutId, string userId);
	Task UpdateWorkout(UpdateWorkoutRequest request, string userId);
	Task<UserAndWorkoutResponse> RestartWorkout(RestartWorkoutRequest request, string userId);
	Task<UserAndWorkoutResponse> RenameWorkout(RenameWorkoutRequest request, string workoutId, string userId);
	Task DeleteWorkout(string workoutId, string userId);
}

public class WorkoutService : IWorkoutService
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;
	private readonly IClock _clock;
	private readonly IWorkoutValidator _workoutValidator;
	private readonly ICommonValidator _commonValidator; // todo delete

	public WorkoutService(IRepository repository, IMapper mapper, IClock clock, IWorkoutValidator workoutValidator,
		ICommonValidator commonValidator)
	{
		_repository = repository;
		_mapper = mapper;
		_clock = clock;
		_workoutValidator = workoutValidator;
		_commonValidator = commonValidator;
	}

	public async Task<UserAndWorkoutResponse> CreateWorkout(CreateWorkoutRequest request, string userId)
	{
		var activeUser = await _repository.GetUser(userId);

		var workoutId = Guid.NewGuid().ToString();
		_workoutValidator.EnsureValidNewWorkout(request, activeUser);

		var creationTime = _clock.GetCurrentInstant().ToString();

		// no error, so go ahead and try and insert this new workout along with updating active user
		var newWorkout = new Workout
		{
			CreationDate = creationTime,
			Creator = userId,
			WorkoutId = workoutId,
			WorkoutName = request.WorkoutName.Trim(),
			Routine = _mapper.Map<Routine>(request.Routine)
		};

		var workoutMeta = new WorkoutMeta
		{
			WorkoutName = request.WorkoutName,
			DateLast = creationTime
		};
		activeUser.Workouts.Add(workoutMeta);
		if (request.SetAsCurrentWorkout)
		{
			activeUser.CurrentWorkout = workoutId;
		}

		// update all the exercises that are now a part of this workout
		WorkoutHelper.UpdateOwnedExercisesOnCreation(activeUser, newWorkout);

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { newWorkout },
			usersToPut: new List<User> { activeUser }
		);

		return new UserAndWorkoutResponse
		{
			User = _mapper.Map<UserResponse>(activeUser),
			Workout = _mapper.Map<WorkoutResponse>(newWorkout)
		};
	}

	public async Task<UserAndWorkoutResponse> SwitchWorkout(SwitchWorkoutRequest request, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var newWorkout = await _repository.GetWorkout(request.NewWorkoutId);
		_workoutValidator.EnsureWorkoutOwnership(userId, newWorkout);
		Workout workoutToSync = null;
		if (request.WorkoutToUpdate != null)
		{
			workoutToSync = _mapper.Map<Workout>(request.WorkoutToUpdate);
			_workoutValidator.EnsureWorkoutOwnership(userId, workoutToSync);
			WorkoutHelper.VerifyCurrentDayAndWeek(workoutToSync);
		}

		activeUser.CurrentWorkout = request.NewWorkoutId;
		var workoutMeta = activeUser.Workouts.First(x => x.WorkoutId == request.NewWorkoutId);
		workoutMeta.DateLast = _clock.GetCurrentInstant().ToString();

		if (workoutToSync != null)
		{
			await _repository.ExecuteBatchWrite(
				usersToPut: new List<User> { activeUser },
				workoutsToPut: new List<Workout> { workoutToSync }
			);
		}
		else
		{
			await _repository.PutUser(activeUser);
		}

		return new UserAndWorkoutResponse
		{
			User = _mapper.Map<UserResponse>(activeUser),
			Workout = _mapper.Map<WorkoutResponse>(newWorkout)
		};
	}

	public async Task<UserAndWorkoutResponse> CopyWorkout(CopyWorkoutRequest request, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var workoutToCopy = await _repository.GetWorkout(request.WorkoutId);
		_workoutValidator.EnsureWorkoutOwnership(userId, workoutToCopy);
		_workoutValidator.EnsureValidCopyWorkout(request, activeUser);

		var newWorkoutId = Guid.NewGuid().ToString();
		var creationTime = _clock.GetCurrentInstant().ToString();
		var newRoutine = workoutToCopy.Routine.Clone();
		var newWorkout = new Workout
		{
			Routine = newRoutine,
			WorkoutName = request.NewName,
			Creator = userId,
			WorkoutId = newWorkoutId,
			CreationDate = creationTime
		};

		activeUser.Workouts.Add(new WorkoutMeta
		{
			WorkoutName = newWorkout.WorkoutName,
			DateLast = creationTime
		});
		// update all the exercises that are now a part of this workout
		WorkoutHelper.UpdateOwnedExercisesOnCreation(activeUser, newWorkout);

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { newWorkout },
			usersToPut: new List<User> { activeUser }
		);

		return new UserAndWorkoutResponse
		{
			User = _mapper.Map<UserResponse>(activeUser),
			Workout = _mapper.Map<WorkoutResponse>(newWorkout)
		};
	}

	public async Task<UserAndWorkoutResponse> SetRoutine(SetRoutineRequest request, string workoutId, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var workout = await _repository.GetWorkout(workoutId);
		var routine = _mapper.Map<Routine>(request.Routine);
		_workoutValidator.EnsureWorkoutOwnership(userId, workout);
		_workoutValidator.EnsureValidEditWorkout(request);

		WorkoutHelper.UpdateOwnedExercisesOnEdit(activeUser, routine, workout);
		workout.Routine = routine;
		WorkoutHelper.VerifyCurrentDayAndWeek(workout);

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { workout },
			usersToPut: new List<User> { activeUser }
		);

		return new UserAndWorkoutResponse
		{
			User = _mapper.Map<UserResponse>(activeUser),
			Workout = _mapper.Map<WorkoutResponse>(workout)
		};
	}

	public async Task UpdateWorkout(UpdateWorkoutRequest request, string userId)
	{
		var workoutToSync = _mapper.Map<Workout>(request.Workout);
		_workoutValidator.EnsureWorkoutOwnership(userId, workoutToSync);
		WorkoutHelper.VerifyCurrentDayAndWeek(workoutToSync);

		await _repository.PutWorkout(workoutToSync);
	}

	public async Task<UserAndWorkoutResponse> RestartWorkout(RestartWorkoutRequest request, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var workout = _mapper.Map<Workout>(request.Workout);
		var workoutMeta = activeUser.Workouts.First(x => x.WorkoutId == request.Workout.WorkoutId);
		_workoutValidator.EnsureWorkoutOwnership(userId, workout);

		WorkoutHelper.RestartWorkout(workout, workoutMeta, activeUser);
		workoutMeta.TimesCompleted += 1;
		workout.CurrentDay = 0;
		workout.CurrentWeek = 0;

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { workout },
			usersToPut: new List<User> { activeUser }
		);

		return new UserAndWorkoutResponse
		{
			User = _mapper.Map<UserResponse>(activeUser),
			Workout = _mapper.Map<WorkoutResponse>(workout)
		};
	}

	public async Task<UserAndWorkoutResponse> RenameWorkout(RenameWorkoutRequest request, string workoutId,
		string userId)
	{
		var workout = await _repository.GetWorkout(workoutId);
		var activeUser = await _repository.GetUser(userId);
		_workoutValidator.EnsureWorkoutOwnership(userId, workout);
		_commonValidator.EnsureValidWorkoutName(request.NewName, activeUser);

		workout.WorkoutName = request.NewName;
		foreach (var exercise in activeUser.Exercises)
		{
			var exerciseWorkout = exercise.Workouts.FirstOrDefault(x => x.WorkoutId == workoutId);
			if (exerciseWorkout != null)
			{
				// old workout name found, replace it with newly named one
				exerciseWorkout.WorkoutName = request.NewName;
			}
		}

		activeUser.Workouts.First(x => x.WorkoutId == workoutId).WorkoutName = request.NewName;

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { workout },
			usersToPut: new List<User> { activeUser }
		);

		return new UserAndWorkoutResponse
		{
			User = _mapper.Map<UserResponse>(activeUser),
			Workout = _mapper.Map<WorkoutResponse>(workout)
		};
	}

	public async Task DeleteWorkout(string workoutId, string userId)
	{
		var workoutToDelete = await _repository.GetWorkout(workoutId);
		var activeUser = await _repository.GetUser(userId);
		_workoutValidator.EnsureWorkoutOwnership(userId, workoutToDelete);

		activeUser.Workouts.RemoveAll(x => x.WorkoutId == workoutId);

		await _repository.ExecuteBatchWrite(
			workoutsToDelete: new List<Workout> { workoutToDelete },
			usersToPut: new List<User> { activeUser }
		);
	}
}