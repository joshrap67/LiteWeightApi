using Google.Cloud.Firestore;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightAPI.Domain.SharedWorkouts;

[FirestoreData]
public class SharedExercise
{
	// public ctor needed for firebase serialization
	public SharedExercise()
	{
	}

	public SharedExercise(RoutineExercise exercise, string exerciseName)
	{
		// todo unit test
		ExerciseName = exerciseName;
		Weight = exercise.Weight;
		Sets = exercise.Sets;
		Reps = exercise.Reps;
		Details = exercise.Details;
	}

	[FirestoreProperty("exerciseName")]
	public string ExerciseName { get; set; }

	[FirestoreProperty("weight")]
	public double Weight { get; set; }

	[FirestoreProperty("sets")]
	public int Sets { get; set; }

	[FirestoreProperty("reps")]
	public int Reps { get; set; }

	[FirestoreProperty("details")]
	public string Details { get; set; }
}