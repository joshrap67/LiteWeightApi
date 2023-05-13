using Google.Cloud.Firestore;

namespace LiteWeightApi.Domain.SharedWorkouts;

[FirestoreData]
public class SharedWeek
{
	[FirestoreProperty("days")] public IList<SharedDay> Days { get; set; } = new List<SharedDay>();

	public void AppendDay(SharedDay sharedDay)
	{
		Days.Add(sharedDay);
	}
}