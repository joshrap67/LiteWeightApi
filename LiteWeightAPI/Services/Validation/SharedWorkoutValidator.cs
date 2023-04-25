using LiteWeightAPI.Api.SharedWorkouts.Requests;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Services.Validation;

public interface ISharedWorkoutValidator
{
	void ValidAcceptSharedWorkout(User activeUser, SharedWorkout sharedWorkout,
		IEnumerable<OwnedExercise> newExercises, string newName);

	void ValidShareWorkout(User activeUser, User recipientUser, Workout workoutToSend, SendWorkoutRequest request);
}

public class SharedWorkoutValidator : ISharedWorkoutValidator
{
	private readonly ICommonValidator _commonValidator;

	public SharedWorkoutValidator(ICommonValidator commonValidator)
	{
		_commonValidator = commonValidator;
	}

	public void ValidShareWorkout(User activeUser, User recipientUser, Workout workoutToSend,
		SendWorkoutRequest request)
	{
		_commonValidator.AuthenticatedUserExists(activeUser);
		_commonValidator.UserExists(recipientUser, request.RecipientUsername);
		_commonValidator.EnsureWorkoutOwnership(activeUser.Username, workoutToSend);

		var senderUsername = activeUser.Username;
		var recipientUsername = recipientUser.Username;
		if (recipientUser.Blocked.Any(x => x.Username == senderUsername))
		{
			throw new UserNotFoundException($"User {request.RecipientUsername} not found");
		}

		if (recipientUser.UserPreferences.PrivateAccount &&
		    !recipientUser.Friends.Any(x => x.Username == senderUsername && x.Confirmed))
		{
			throw new UserNotFoundException($"User {request.RecipientUsername} not found");
		}

		if (recipientUser.ReceivedWorkouts.Count >= Globals.MaxReceivedWorkouts)
		{
			throw new MaximumReachedException($"{recipientUsername} has too many received workouts");
		}

		if (activeUser.WorkoutsSent >= Globals.MaxFreeWorkoutsSent)
		{
			throw new MaximumReachedException("You have reached the maximum number of workouts that you can send");
		}

		if (senderUsername == recipientUsername)
		{
			throw new MiscErrorException("Cannot send workout to yourself");
		}
	}

	public void ValidAcceptSharedWorkout(User activeUser, SharedWorkout sharedWorkout,
		IEnumerable<OwnedExercise> newExercises, string newName)
	{
		_commonValidator.AuthenticatedUserExists(activeUser);
		_commonValidator.EnsureSharedWorkoutResourceExists(sharedWorkout);

		if (activeUser.PremiumToken == null && activeUser.Workouts.Count >= Globals.MaxFreeWorkouts)
		{
			throw new MaximumReachedException("Maximum workouts would be exceeded");
		}

		if (activeUser.PremiumToken != null && activeUser.Workouts.Count >= Globals.MaxWorkouts)
		{
			throw new MaximumReachedException("Maximum workouts would be exceeded");
		}

		if (newName != null)
		{
			_commonValidator.EnsureValidWorkoutName(newName, activeUser);
		}

		var workoutMetas = new List<WorkoutMeta>(activeUser.Workouts);
		if (workoutMetas.Any(workoutMeta => workoutMeta.WorkoutName == sharedWorkout.WorkoutName))
		{
			throw new DuplicateFoundException("Workout with this name already exists");
		}

		var ownedExercises = new HashSet<string>();
		foreach (var exercise in activeUser.Exercises)
		{
			ownedExercises.Add(exercise.ExerciseName);
		}

		var newExercisesHashSet = newExercises.Select(x => x.ExerciseName).ToHashSet();
		var totalExercises = newExercisesHashSet.Union(ownedExercises).ToList();
		if (activeUser.PremiumToken == null && totalExercises.Count > Globals.MaxFreeExercises)
		{
			throw new MaximumReachedException(
				"Accepting this workout would put you above the amount of exercises allowed");
		}

		if (activeUser.PremiumToken != null && totalExercises.Count > Globals.MaxPremiumExercises)
		{
			throw new MaximumReachedException(
				"Accepting this workout would put you above the amount of exercises allowed");
		}
	}
}