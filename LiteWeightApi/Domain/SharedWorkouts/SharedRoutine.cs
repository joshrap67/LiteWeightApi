using Google.Cloud.Firestore;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightAPI.Domain.SharedWorkouts;

[FirestoreData]
public class SharedRoutine
{
	// public ctor needed for firebase serialization
	public SharedRoutine() { }

	public SharedRoutine(Routine routine, IEnumerable<OwnedExercise> ownedExercises)
	{
		var exerciseIdToExercise = ownedExercises.ToDictionary(x => x.Id, x => x);
		Weeks = new List<SharedWeek>();
		foreach (var week in routine.Weeks)
		{
			var sharedWeek = new SharedWeek();
			foreach (var day in week.Days)
			{
				var sharedDay = new SharedDay { Tag = day.Tag };
				foreach (var exercise in day.Exercises)
				{
					var ownedExercise = exerciseIdToExercise[exercise.ExerciseId];
					var sharedExercise = new SharedExercise(exercise, ownedExercise.Name);
					sharedDay.AppendExercise(sharedExercise);
				}

				sharedWeek.AppendDay(sharedDay);
			}

			AppendWeek(sharedWeek);
		}
	}

	[FirestoreProperty("weeks")]
	public IList<SharedWeek> Weeks { get; set; }

	private void AppendWeek(SharedWeek sharedWeek)
	{
		Weeks.Add(sharedWeek);
	}
}