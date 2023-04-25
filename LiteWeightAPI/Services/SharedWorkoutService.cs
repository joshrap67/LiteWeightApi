using AutoMapper;
using LiteWeightAPI.Api.SharedWorkouts.Requests;
using LiteWeightAPI.Api.SharedWorkouts.Responses;
using LiteWeightAPI.Api.Users.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Services.Helpers;
using LiteWeightAPI.Services.Notifications;
using LiteWeightAPI.Services.Validation;
using NodaTime;

namespace LiteWeightAPI.Services;

public interface ISharedWorkoutService
{
	Task<string> ShareWorkout(SendWorkoutRequest request, string userId);
	Task<SharedWorkoutResponse> GetSharedWorkout(string sharedWorkoutId);
	Task<AcceptSharedWorkoutResponse> AcceptWorkout(string sharedWorkoutId, string userId, string newName = null);
	Task DeclineWorkout(string sharedWorkoutId, string userId);
}

public class SharedWorkoutService : ISharedWorkoutService
{
	private readonly IRepository _repository;
	private readonly IClock _clock;
	private readonly ISharedWorkoutValidator _sharedWorkoutValidator;
	private readonly IMapper _mapper;
	private readonly IPushNotificationService _pushNotificationService;

	public SharedWorkoutService(IRepository repository, IClock clock,
		ISharedWorkoutValidator sharedWorkoutValidator, IMapper mapper,
		IPushNotificationService pushNotificationService)
	{
		_repository = repository;
		_clock = clock;
		_sharedWorkoutValidator = sharedWorkoutValidator;
		_mapper = mapper;
		_pushNotificationService = pushNotificationService;
	}

	public async Task<string> ShareWorkout(SendWorkoutRequest request, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var recipientUser = await _repository.GetUser(request.RecipientUsername);
		var workoutToSend = await _repository.GetWorkout(request.WorkoutId);
		_sharedWorkoutValidator.ValidShareWorkout(activeUser, recipientUser, workoutToSend, request);

		var sharedWorkoutId = Guid.NewGuid().ToString();
		var sharedWorkoutInfo = new SharedWorkoutMeta
		{
			WorkoutId = sharedWorkoutId,
			Icon = activeUser.Icon,
			Sender = userId,
			WorkoutName = workoutToSend.WorkoutName,
			DateSent = _clock.GetCurrentInstant().ToString(),
			TotalDays = workoutToSend.Routine.TotalNumberOfDays,
			MostFrequentFocus = WorkoutHelper.FindMostFrequentFocus(activeUser, workoutToSend.Routine)
		};
		var sharedWorkout = new SharedWorkout(workoutToSend, sharedWorkoutId, activeUser);
		recipientUser.ReceivedWorkouts.Add(sharedWorkoutInfo);
		activeUser.WorkoutsSent++;

		await _repository.ExecuteBatchWrite(
			usersToPut: new List<User> { activeUser, recipientUser },
			sharedWorkoutsToPut: new List<SharedWorkout> { sharedWorkout }
		);

		await _pushNotificationService.SendWorkoutPushNotification(recipientUser, sharedWorkoutInfo);

		return sharedWorkoutId;
	}

	public async Task<SharedWorkoutResponse> GetSharedWorkout(string sharedWorkoutId)
	{
		var sharedWorkout = await _repository.GetSharedWorkout(sharedWorkoutId);
		if (sharedWorkout == null)
		{
			throw new ResourceNotFoundException("Shared workout not found");
		}

		return _mapper.Map<SharedWorkoutResponse>(sharedWorkout);
	}

	public async Task<AcceptSharedWorkoutResponse> AcceptWorkout(string sharedWorkoutId, string userId,
		string newName = null)
	{
		var activeUser = await _repository.GetUser(userId);
		var workoutToAccept = await _repository.GetSharedWorkout(sharedWorkoutId);
		var newExercises = SharedWorkoutHelper.GetNewExercisesFromSharedWorkout(workoutToAccept, activeUser).ToList();
		_sharedWorkoutValidator.ValidAcceptSharedWorkout(activeUser, workoutToAccept, newExercises, newName);

		if (newName != null)
		{
			workoutToAccept.WorkoutName = newName;
		}

		foreach (var newExercise in newExercises)
		{
			activeUser.Exercises.Add(newExercise);
		}

		var exerciseNameToId = activeUser.Exercises.ToDictionary(x => x.ExerciseName, y => y.Id);
		var newWorkoutId = Guid.NewGuid().ToString();
		var creationTime = _clock.GetCurrentInstant().ToString();
		var newWorkout = new Workout
		{
			WorkoutName = workoutToAccept.WorkoutName,
			Creator = userId,
			CreationDate = creationTime,
			WorkoutId = newWorkoutId,
			Routine = new Routine(workoutToAccept.Routine, exerciseNameToId)
		};

		var workoutMeta = new WorkoutMeta
		{
			DateLast = creationTime,
			WorkoutName = newWorkout.WorkoutName,
			WorkoutId = newWorkoutId
		};
		activeUser.Workouts.Add(workoutMeta);
		WorkoutHelper.UpdateOwnedExercisesOnCreation(activeUser, newWorkout);

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { newWorkout },
			usersToPut: new List<User> { activeUser },
			sharedWorkoutsToDelete: new List<SharedWorkout> { workoutToAccept }
		);

		return new AcceptSharedWorkoutResponse
		{
			NewWorkoutInfo = _mapper.Map<WorkoutMetaResponse>(workoutMeta),
			NewExercises = _mapper.Map<IList<OwnedExerciseResponse>>(activeUser.Exercises)
		};
	}

	public async Task DeclineWorkout(string sharedWorkoutId, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var workoutToDecline = await _repository.GetSharedWorkout(sharedWorkoutId);
		if (workoutToDecline == null)
		{
			throw new ResourceNotFoundException("Shared workout not found");
		}

		activeUser.ReceivedWorkouts.RemoveAll(x => x.WorkoutId == sharedWorkoutId);

		await _repository.ExecuteBatchWrite(
			usersToPut: new List<User> { activeUser },
			sharedWorkoutsToDelete: new List<SharedWorkout> { workoutToDecline }
		);
	}
}