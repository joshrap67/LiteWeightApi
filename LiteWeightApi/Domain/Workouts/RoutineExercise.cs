using Google.Cloud.Firestore;

namespace LiteWeightApi.Domain.Workouts;

[FirestoreData]
public class RoutineExercise
{
	[FirestoreProperty("completed")] public bool Completed { get; set; }
	[FirestoreProperty("exerciseId")] public string ExerciseId { get; set; }
	[FirestoreProperty("weight")] public double Weight { get; set; }
	[FirestoreProperty("sets")] public int Sets { get; set; }
	[FirestoreProperty("reps")] public int Reps { get; set; }
	[FirestoreProperty("details")] public string Details { get; set; }

	public RoutineExercise Clone()
	{
		var copy = new RoutineExercise
		{
			Completed = Completed,
			Details = Details,
			Reps = Reps,
			Sets = Sets,
			Weight = Weight,
			ExerciseId = ExerciseId
		};
		return copy;
	}
}