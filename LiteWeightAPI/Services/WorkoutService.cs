using AutoMapper;
using LiteWeightAPI.Api.Common.Responses;
using LiteWeightAPI.Api.CurrentUser.Responses;
using LiteWeightAPI.Api.Workouts.Requests;
using LiteWeightAPI.Api.Workouts.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Services.Helpers;
using LiteWeightAPI.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace LiteWeightAPI.Services;

public interface IWorkoutService
{
	Task<UserAndWorkoutResponse> CreateWorkout(CreateWorkoutRequest request, string userId);
	Task<ActionResult<WorkoutResponse>> GetWorkout(string workoutId, string currentUserId);
	Task<UserAndWorkoutResponse> CopyWorkout(CopyWorkoutRequest request, string workoutId, string userId);
	Task<UserAndWorkoutResponse> SetRoutine(SetRoutineRequest request, string workoutId, string userId);
	Task UpdateWorkout(string workoutId, UpdateWorkoutRequest request, string userId);
	Task<UserAndWorkoutResponse> RestartWorkout(string workoutId, RestartWorkoutRequest request, string userId);
	Task RenameWorkout(RenameWorkoutRequest request, string workoutId, string userId);
	Task DeleteWorkout(string workoutId, string userId);
	Task ResetStatistics(string workoutId, string userId);
}

public class WorkoutService : IWorkoutService
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;
	private readonly IClock _clock;
	private readonly IWorkoutValidator _workoutValidator;

	public WorkoutService(IRepository repository, IMapper mapper, IClock clock, IWorkoutValidator workoutValidator)
	{
		_repository = repository;
		_mapper = mapper;
		_clock = clock;
		_workoutValidator = workoutValidator;
	}

	public async Task<UserAndWorkoutResponse> CreateWorkout(CreateWorkoutRequest request, string userId)
	{
		var user = await _repository.GetUser(userId);

		var workoutId = Guid.NewGuid().ToString();
		_workoutValidator.ValidCreateWorkout(request, user);

		var now = _clock.GetCurrentInstant();
		var newWorkout = new Workout
		{
			Id = workoutId,
			Name = request.WorkoutName.Trim(),
			CreationUtc = now,
			CreatorId = userId,
			Routine = _mapper.Map<Routine>(request.Routine)
		};

		var workoutMeta = new WorkoutInfo
		{
			WorkoutId = workoutId,
			WorkoutName = request.WorkoutName
		};
		user.Workouts.Add(workoutMeta);
		if (request.SetAsCurrentWorkout)
		{
			user.CurrentWorkoutId = workoutId;
		}

		// update all the exercises that are now part of this workout
		WorkoutHelper.UpdateOwnedExercisesOnCreation(user, newWorkout);

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { newWorkout },
			usersToPut: new List<User> { user }
		);

		return new UserAndWorkoutResponse
		{
			User = _mapper.Map<UserResponse>(user),
			Workout = _mapper.Map<WorkoutResponse>(newWorkout)
		};
	}

	public async Task<ActionResult<WorkoutResponse>> GetWorkout(string workoutId, string currentUserId)
	{
		var workout = await _repository.GetWorkout(workoutId);
		if (workout == null)
		{
			throw new ResourceNotFoundException("Workout");
		}

		return _mapper.Map<WorkoutResponse>(workout);
	}

	public async Task<UserAndWorkoutResponse> CopyWorkout(CopyWorkoutRequest request, string workoutId, string userId)
	{
		var user = await _repository.GetUser(userId);
		var workoutToCopy = await _repository.GetWorkout(workoutId);
		_workoutValidator.ValidCopyWorkout(request, workoutToCopy, user);

		var newWorkoutId = Guid.NewGuid().ToString();
		var now = _clock.GetCurrentInstant();
		var newRoutine = workoutToCopy.Routine.Clone();
		var newWorkout = new Workout
		{
			Id = newWorkoutId,
			Name = request.NewName,
			Routine = newRoutine,
			CreatorId = userId,
			CreationUtc = now
		};

		user.Workouts.Add(new WorkoutInfo
		{
			WorkoutId = newWorkoutId,
			WorkoutName = newWorkout.Name
		});
		// update all the exercises that are now part of this workout
		WorkoutHelper.UpdateOwnedExercisesOnCreation(user, newWorkout);

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { newWorkout },
			usersToPut: new List<User> { user }
		);

		return new UserAndWorkoutResponse
		{
			User = _mapper.Map<UserResponse>(user),
			Workout = _mapper.Map<WorkoutResponse>(newWorkout)
		};
	}

	public async Task<UserAndWorkoutResponse> SetRoutine(SetRoutineRequest request, string workoutId, string userId)
	{
		var user = await _repository.GetUser(userId);
		var workout = await _repository.GetWorkout(workoutId);
		var routine = _mapper.Map<Routine>(request);
		_workoutValidator.ValidSetRoutine(workout, request, user);

		WorkoutHelper.UpdateOwnedExercisesOnEdit(user, routine, workout);
		workout.Routine = routine;
		WorkoutHelper.FixCurrentDayAndWeek(workout);

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { workout },
			usersToPut: new List<User> { user }
		);

		return new UserAndWorkoutResponse
		{
			User = _mapper.Map<UserResponse>(user),
			Workout = _mapper.Map<WorkoutResponse>(workout)
		};
	}

	public async Task UpdateWorkout(string workoutId, UpdateWorkoutRequest request, string userId)
	{
		var user = await _repository.GetUser(userId);
		var workoutToUpdate = await _repository.GetWorkout(workoutId);
		_workoutValidator.ValidUpdateWorkout(workoutToUpdate, user);

		var routine = _mapper.Map<Routine>(request.Routine);
		workoutToUpdate.Routine = routine;
		workoutToUpdate.CurrentDay = request.CurrentDay;
		workoutToUpdate.CurrentWeek = request.CurrentWeek;

		WorkoutHelper.FixCurrentDayAndWeek(workoutToUpdate);

		await _repository.PutWorkout(workoutToUpdate);
	}

	public async Task<UserAndWorkoutResponse> RestartWorkout(string workoutId, RestartWorkoutRequest request,
		string userId)
	{
		var user = await _repository.GetUser(userId);
		var workout = await _repository.GetWorkout(workoutId);
		_workoutValidator.ValidRestartWorkout(workout, user);

		var routine = _mapper.Map<Routine>(request.Routine);
		var workoutMeta = user.Workouts.First(x => x.WorkoutId == workoutId);
		WorkoutHelper.RestartWorkout(routine, workoutMeta, user);
		workoutMeta.TimesCompleted += 1;
		workout.CurrentDay = 0;
		workout.CurrentWeek = 0;

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { workout },
			usersToPut: new List<User> { user }
		);

		return new UserAndWorkoutResponse
		{
			User = _mapper.Map<UserResponse>(user),
			Workout = _mapper.Map<WorkoutResponse>(workout)
		};
	}

	public async Task RenameWorkout(RenameWorkoutRequest request, string workoutId,
		string userId)
	{
		var workout = await _repository.GetWorkout(workoutId);
		var user = await _repository.GetUser(userId);
		_workoutValidator.ValidRenameWorkout(request, workout, user);

		var newName = request.NewName;
		workout.Name = newName;
		foreach (var exercise in user.Exercises)
		{
			var exerciseWorkout = exercise.Workouts.FirstOrDefault(x => x.WorkoutId == workoutId);
			if (exerciseWorkout != null)
			{
				// old workout name found, replace it with newly named one
				exerciseWorkout.WorkoutName = newName;
			}
		}

		user.Workouts.First(x => x.WorkoutId == workoutId).WorkoutName = newName;

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { workout },
			usersToPut: new List<User> { user }
		);
	}

	public async Task ResetStatistics(string workoutId, string userId)
	{
		var user = await _repository.GetUser(userId);
		_workoutValidator.ValidResetStatistics(user, workoutId);

		var workoutInfo = user.Workouts.First(x => x.WorkoutId == workoutId);
		workoutInfo.TimesCompleted = 0;
		workoutInfo.AverageExercisesCompleted = 0.0;
		workoutInfo.TotalExercisesSum = 0;

		await _repository.PutUser(user);
	}

	public async Task DeleteWorkout(string workoutId, string userId)
	{
		var workoutToDelete = await _repository.GetWorkout(workoutId);
		var user = await _repository.GetUser(userId);
		_workoutValidator.ValidDeleteWorkout(workoutToDelete, user);

		user.Workouts.RemoveAll(x => x.WorkoutId == workoutId);
		foreach (var ownedExercise in user.Exercises)
		{
			var ownedExerciseWorkout = ownedExercise.Workouts.FirstOrDefault(x => x.WorkoutId == workoutId);
			if (ownedExerciseWorkout != null)
			{
				ownedExercise.Workouts.Remove(ownedExerciseWorkout);
			}
		}

		await _repository.ExecuteBatchWrite(
			workoutsToDelete: new List<Workout> { workoutToDelete },
			usersToPut: new List<User> { user }
		);
	}
}