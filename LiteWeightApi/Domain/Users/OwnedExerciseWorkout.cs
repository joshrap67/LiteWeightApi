using Google.Cloud.Firestore;

namespace LiteWeightAPI.Domain.Users;

[FirestoreData]
public class OwnedExerciseWorkout
{
	[FirestoreProperty("workoutId")]
	public string WorkoutId { get; set; }

	[FirestoreProperty("workoutName")]
	public string WorkoutName { get; set; }
}