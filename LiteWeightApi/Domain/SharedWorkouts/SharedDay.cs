using Google.Cloud.Firestore;

namespace LiteWeightApi.Domain.SharedWorkouts;

[FirestoreData]
public class SharedDay
{
	[FirestoreProperty("exercises")] public IList<SharedExercise> Exercises { get; set; } = new List<SharedExercise>();
	[FirestoreProperty("tag")] public string Tag { get; set; }

	public void AppendExercise(SharedExercise sharedExercise)
	{
		Exercises.Add(sharedExercise);
	}
}