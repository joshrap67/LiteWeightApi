using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Services.Validation;

public interface ISharedWorkoutValidator
{
	void ValidGetSharedWorkout(SharedWorkout sharedWorkout, string userId);

	void ValidAcceptSharedWorkout(User user, SharedWorkout sharedWorkout, IEnumerable<OwnedExercise> newExercises,
		string newName);

	void ValidDeclineSharedWorkout(SharedWorkout sharedWorkout, string userId);
}

public class SharedWorkoutValidator : ISharedWorkoutValidator
{
	private readonly ICommonValidator _commonValidator;

	public SharedWorkoutValidator(ICommonValidator commonValidator)
	{
		_commonValidator = commonValidator;
	}

	public void ValidGetSharedWorkout(SharedWorkout sharedWorkout, string userId)
	{
		_commonValidator.SharedWorkoutExists(sharedWorkout);
		_commonValidator.EnsureSharedWorkoutOwnership(userId, sharedWorkout);
	}

	public void ValidAcceptSharedWorkout(User user, SharedWorkout sharedWorkout,
		IEnumerable<OwnedExercise> newExercises, string newName)
	{
		_commonValidator.SharedWorkoutExists(sharedWorkout);
		_commonValidator.EnsureSharedWorkoutOwnership(user.Id, sharedWorkout);

		if (user.PremiumToken == null && user.Workouts.Count >= Globals.MaxFreeWorkouts)
		{
			throw new MaxLimitException("Maximum workouts would be exceeded");
		}

		if (user.PremiumToken != null && user.Workouts.Count >= Globals.MaxWorkouts)
		{
			throw new MaxLimitException("Maximum workouts would be exceeded");
		}

		_commonValidator.ValidWorkoutName(newName ?? sharedWorkout.WorkoutName, user);

		var workoutMetas = new List<WorkoutInfo>(user.Workouts);
		if (workoutMetas.Any(workoutMeta => workoutMeta.WorkoutName == sharedWorkout.WorkoutName))
		{
			throw new AlreadyExistsException("Workout with this name already exists");
		}

		var ownedExercises = new HashSet<string>();
		foreach (var exercise in user.Exercises)
		{
			ownedExercises.Add(exercise.Name);
		}

		var newExercisesHashSet = newExercises.Select(x => x.Name).ToHashSet();
		var totalExercises = newExercisesHashSet.Union(ownedExercises).ToList();
		if (user.PremiumToken == null && totalExercises.Count > Globals.MaxFreeExercises)
		{
			throw new MaxLimitException(
				"Accepting this workout would put you above the amount of exercises allowed");
		}

		if (user.PremiumToken != null && totalExercises.Count > Globals.MaxPremiumExercises)
		{
			throw new MaxLimitException(
				"Accepting this workout would put you above the amount of exercises allowed");
		}
	}

	public void ValidDeclineSharedWorkout(SharedWorkout sharedWorkout, string userId)
	{
		_commonValidator.EnsureSharedWorkoutOwnership(userId, sharedWorkout);
	}
}