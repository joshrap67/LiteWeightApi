using Google.Cloud.Firestore;

namespace LiteWeightApi.Domain.Workouts;

[FirestoreData]
public class RoutineWeek
{
	[FirestoreProperty("days")] public IList<RoutineDay> Days { get; set; } = new List<RoutineDay>();

	public void AppendDay(RoutineDay routineDay)
	{
		Days.Add(routineDay);
	}
}