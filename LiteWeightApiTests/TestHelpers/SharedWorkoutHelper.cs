using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightApiTests.TestHelpers;

public static class SharedWorkoutHelper
{
	private static readonly Fixture Fixture = new();

	public static SharedWorkout GetSharedWorkout(string recipientId = null, string sharedWorkoutId = null)
	{
		var workout = Fixture.Create<Workout>();
		var workoutExerciseIds = workout.Routine.Weeks.SelectMany(x => x.Days).SelectMany(x => x.Exercises)
			.Select(x => x.ExerciseId).ToList();
		var exercises = workoutExerciseIds
			.Select(exerciseId => Fixture.Build<OwnedExercise>().With(x => x.Id, exerciseId).Create()).ToList();

		var sender = Fixture.Build<User>()
			.With(x => x.Exercises, exercises)
			.Create();

		return new SharedWorkout(workout, recipientId ?? Fixture.Create<string>(),
			sharedWorkoutId ?? Fixture.Create<string>(), sender);
	}
}