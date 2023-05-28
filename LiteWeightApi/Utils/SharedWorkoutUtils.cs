using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;

namespace LiteWeightAPI.Utils;

public static class SharedWorkoutUtils
{
	public static IEnumerable<OwnedExercise> GetNewExercisesFromSharedWorkout(SharedWorkout sharedWorkout, User user)
	{
		if (sharedWorkout == null || user == null)
		{
			return new List<OwnedExercise>();
		}

		var newExercises = new List<OwnedExercise>();
		var sharedExerciseNames = sharedWorkout.Routine.Weeks
			.SelectMany(x => x.Days)
			.SelectMany(x => x.Exercises)
			.Select(x => x.ExerciseName)
			.ToHashSet();

		var ownedExerciseNames = user.Exercises.Select(x => x.Name).ToHashSet();

		sharedExerciseNames.ExceptWith(ownedExerciseNames);

		foreach (var exerciseName in sharedExerciseNames)
		{
			// for each of the exercises that the user doesn't own, make a new entry for them
			var sharedExercise = sharedWorkout.DistinctExercises.First(x => x.ExerciseName == exerciseName);
			var ownedExercise = new OwnedExercise
			{
				Name = exerciseName,
				Focuses = sharedExercise.Focuses,
				VideoUrl = sharedExercise.VideoUrl
			};
			newExercises.Add(ownedExercise);
		}

		return newExercises;
	}
}