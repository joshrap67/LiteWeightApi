using Google.Cloud.Firestore;
using LiteWeightApi.Domain.SharedWorkouts;

namespace LiteWeightApi.Domain.Workouts;

[FirestoreData]
public class Routine
{
	public Routine()
	{
	}

	public Routine(SharedRoutine routine, IReadOnlyDictionary<string, string> exerciseNameToId)
	{
		// constructor is used to convert from a shared routine back to a normal workout routine
		Weeks = new List<RoutineWeek>();
		foreach (var week in routine.Weeks)
		{
			var routineWeek = new RoutineWeek();
			foreach (var day in week.Days)
			{
				var routineDay = new RoutineDay
				{
					Tag = day.Tag
				};
				foreach (var sharedExercise in day.Exercises)
				{
					var routineExercise = new RoutineExercise
					{
						ExerciseId = exerciseNameToId[sharedExercise.ExerciseName],
						Weight = sharedExercise.Weight,
						Sets = sharedExercise.Reps,
						Reps = sharedExercise.Reps,
						Details = sharedExercise.Details
					};
					routineDay.AppendExercise(routineExercise);
				}

				routineWeek.AppendDay(routineDay);
			}

			AppendWeek(routineWeek);
		}
	}

	[FirestoreProperty("weeks")] public IList<RoutineWeek> Weeks { get; set; } = new List<RoutineWeek>();

	public int TotalNumberOfDays => Weeks.Sum(x => x.Days.Count);

	public void AppendWeek(RoutineWeek week)
	{
		Weeks.Add(week);
	}

	public void DeleteExerciseFromRoutine(string exerciseId)
	{
		foreach (var week in Weeks)
		{
			foreach (var day in week.Days)
			{
				day.DeleteExercise(exerciseId);
			}
		}
	}

	public Routine Clone()
	{
		var copy = new Routine
		{
			Weeks = new List<RoutineWeek>()
		};
		foreach (var week in Weeks)
		{
			var routineWeek = new RoutineWeek();
			foreach (var day in week.Days)
			{
				routineWeek.AppendDay(day.Clone());
			}

			copy.Weeks.Add(routineWeek);
		}

		return copy;
	}
}