using AutoMapper;
using LiteWeightApi.Api.CurrentUser.Responses;
using LiteWeightApi.Api.Exercises.Responses;
using LiteWeightApi.Api.SharedWorkouts.Requests;
using LiteWeightApi.Api.SharedWorkouts.Responses;
using LiteWeightApi.Domain;
using LiteWeightApi.Domain.SharedWorkouts;
using LiteWeightApi.Domain.Users;
using LiteWeightApi.Domain.Workouts;
using LiteWeightApi.Services.Helpers;
using LiteWeightApi.Services.Validation;
using NodaTime;

namespace LiteWeightApi.Services;

public interface ISharedWorkoutService
{
	Task<SharedWorkoutResponse> GetSharedWorkout(string sharedWorkoutId, string userId);

	Task<AcceptSharedWorkoutResponse> AcceptWorkout(string sharedWorkoutId, string userId,
		AcceptSharedWorkoutRequest request);

	Task DeclineWorkout(string sharedWorkoutId, string userId);
}

public class SharedWorkoutService : ISharedWorkoutService
{
	private readonly IRepository _repository;
	private readonly IClock _clock;
	private readonly ISharedWorkoutValidator _sharedWorkoutValidator;
	private readonly IMapper _mapper;

	public SharedWorkoutService(IRepository repository, IClock clock, ISharedWorkoutValidator sharedWorkoutValidator,
		IMapper mapper)
	{
		_repository = repository;
		_clock = clock;
		_sharedWorkoutValidator = sharedWorkoutValidator;
		_mapper = mapper;
	}

	public async Task<SharedWorkoutResponse> GetSharedWorkout(string sharedWorkoutId, string userId)
	{
		var sharedWorkout = await _repository.GetSharedWorkout(sharedWorkoutId);
		_sharedWorkoutValidator.ValidGetSharedWorkout(sharedWorkout, userId);

		return _mapper.Map<SharedWorkoutResponse>(sharedWorkout);
	}

	public async Task<AcceptSharedWorkoutResponse> AcceptWorkout(string sharedWorkoutId, string userId,
		AcceptSharedWorkoutRequest request)
	{
		var user = await _repository.GetUser(userId);
		var workoutToAccept = await _repository.GetSharedWorkout(sharedWorkoutId);
		var newExercises = SharedWorkoutHelper.GetNewExercisesFromSharedWorkout(workoutToAccept, user).ToList();
		_sharedWorkoutValidator.ValidAcceptSharedWorkout(user, workoutToAccept, newExercises, request.NewName);

		if (request.NewName != null)
		{
			workoutToAccept.WorkoutName = request.NewName;
		}

		foreach (var newExercise in newExercises)
		{
			user.Exercises.Add(newExercise);
		}

		var exerciseNameToId = user.Exercises.ToDictionary(x => x.Name, y => y.Id);
		var newWorkoutId = Guid.NewGuid().ToString();
		var now = _clock.GetCurrentInstant();
		var newWorkout = new Workout
		{
			Id = newWorkoutId,
			Name = workoutToAccept.WorkoutName,
			CreatorId = userId,
			CreationUtc = now,
			Routine = new Routine(workoutToAccept.Routine, exerciseNameToId)
		};

		var workoutMeta = new WorkoutInfo
		{
			LastSetAsCurrentUtc = now,
			WorkoutName = newWorkout.Name,
			WorkoutId = newWorkoutId
		};
		user.Workouts.Add(workoutMeta);
		WorkoutHelper.UpdateOwnedExercisesOnCreation(user, newWorkout);

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { newWorkout },
			usersToPut: new List<User> { user },
			sharedWorkoutsToDelete: new List<SharedWorkout> { workoutToAccept }
		);

		return new AcceptSharedWorkoutResponse
		{
			NewWorkoutInfo = _mapper.Map<WorkoutInfoResponse>(workoutMeta),
			NewExercises = _mapper.Map<IList<OwnedExerciseResponse>>(user.Exercises)
		};
	}

	public async Task DeclineWorkout(string sharedWorkoutId, string userId)
	{
		var user = await _repository.GetUser(userId);
		var workoutToDecline = await _repository.GetSharedWorkout(sharedWorkoutId);

		_sharedWorkoutValidator.ValidDeclineSharedWorkout(workoutToDecline, userId);

		user.ReceivedWorkouts.RemoveAll(x => x.SharedWorkoutId == sharedWorkoutId);

		await _repository.ExecuteBatchWrite(
			usersToPut: new List<User> { user },
			sharedWorkoutsToDelete: new List<SharedWorkout> { workoutToDecline }
		);
	}
}