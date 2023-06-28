using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightApiTests.Domain.SharedWorkouts;

public class SharedWorkoutTests : BaseTest
{
	[Fact]
	public void Ctor_Success()
	{
		var workout = Fixture.Create<Workout>();
		var distinctExercises = workout.Routine.Weeks
			.SelectMany(x => x.Days)
			.SelectMany(x => x.Exercises)
			.ToList();
		var ownedExercises = new List<OwnedExercise>();
		foreach (var exerciseId in distinctExercises.Select(x => x.ExerciseId))
		{
			var exercise = Fixture.Build<OwnedExercise>().With(x => x.Id, exerciseId).Create();
			ownedExercises.Add(exercise);
		}

		var sender = Fixture.Build<User>().With(x => x.Exercises, ownedExercises).Create();
		var sharedWorkoutId = Fixture.Create<string>();
		var recipientId = Fixture.Create<string>();

		var sharedWorkout = new SharedWorkout(workout, recipientId, sharedWorkoutId, sender);

		Assert.Equal(recipientId, sharedWorkout.RecipientId);
		Assert.Equal(sender.Id, sharedWorkout.SenderId);
		Assert.Equal(sender.Username, sharedWorkout.SenderUsername);
		Assert.Equal(workout.Name, sharedWorkout.WorkoutName);
		Assert.Equal(sharedWorkoutId, sharedWorkout.Id);

		var exerciseNameToExercise = ownedExercises.ToDictionary(x => x.Name, x => x);
		// var expectedDistinctHashSet = ownedExercises.Select(x => x.Name).ToHashSet();
		// var actualDistinctHashSet = sharedWorkout.DistinctExercises.Select(x => x.ExerciseName).ToHashSet();
		foreach (var distinctExercise in sharedWorkout.DistinctExercises)
		{
			var ownedExercise = exerciseNameToExercise[distinctExercise.ExerciseName];
			Assert.Equal(ownedExercise.VideoUrl, distinctExercise.VideoUrl);
			Assert.Equal(ownedExercise.Focuses, distinctExercise.Focuses);
		}
		// Assert.True(expectedDistinctHashSet.SetEquals(actualDistinctHashSet));

		for (var i = 0; i < sharedWorkout.Routine.Weeks.Count; i++)
		{
			var expectedWeek = workout.Routine.Weeks[i];
			var actualWeek = sharedWorkout.Routine.Weeks[i];
			Assert.Equal(expectedWeek.Days.Count, actualWeek.Days.Count);
			for (var j = 0; j < actualWeek.Days.Count; j++)
			{
				var expectedDay = expectedWeek.Days[j];
				var actualDay = actualWeek.Days[j];
				Assert.Equal(expectedDay.Tag, actualDay.Tag);
				Assert.Equal(expectedDay.Exercises.Count, actualDay.Exercises.Count);
				for (var k = 0; k < actualDay.Exercises.Count; k++)
				{
					var expectedExercise = expectedDay.Exercises[k];
					var actualExercise = actualDay.Exercises[k];
					var id = exerciseNameToExercise[actualExercise.ExerciseName].Id;
					Assert.Equal(expectedExercise.ExerciseId, id);
					Assert.Equal(expectedExercise.Weight, actualExercise.Weight);
					Assert.Equal(expectedExercise.Sets, actualExercise.Sets);
					Assert.Equal(expectedExercise.Reps, actualExercise.Reps);
					Assert.Equal(expectedExercise.Details, actualExercise.Details);
				}
			}
		}
	}
}