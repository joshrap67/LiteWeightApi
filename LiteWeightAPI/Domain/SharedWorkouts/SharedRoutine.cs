using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightAPI.Domain.SharedWorkouts;

public class SharedRoutine
{
	public SharedRoutine(Routine routine, IList<OwnedExercise> ownedExercises)
	{
		Weeks = new List<SharedWeek>();
		foreach (var week in routine.Weeks)
		{
			var sharedWeek = new SharedWeek();
			foreach (var day in week.Days)
			{
				var sharedDay = new SharedDay
				{
					Tag = day.Tag
				};
				foreach (var exercise in day.Exercises)
				{
					var ownedExercise = ownedExercises.First(x => x.Id == exercise.ExerciseId);
					var sharedExercise = new SharedExercise(exercise, ownedExercise.ExerciseName);
					sharedDay.AppendExercise(sharedExercise);
				}

				sharedWeek.AppendDay(sharedDay);
			}

			AppendWeek(sharedWeek);
		}
	}

	public IList<SharedWeek> Weeks { get; set; }

	private void AppendWeek(SharedWeek sharedWeek)
	{
		Weeks.Add(sharedWeek);
	}
}