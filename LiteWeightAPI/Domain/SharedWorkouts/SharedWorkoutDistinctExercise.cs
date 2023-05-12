using Google.Cloud.Firestore;
using LiteWeightAPI.Domain.Users;

namespace LiteWeightAPI.Domain.SharedWorkouts;

[FirestoreData]
public class SharedWorkoutDistinctExercise
{
	public SharedWorkoutDistinctExercise(OwnedExercise userExercise, string exerciseName)
	{
		ExerciseName = exerciseName;
		VideoUrl = userExercise.VideoUrl;
		Focuses = userExercise.Focuses;
	}

	[FirestoreProperty("exerciseName")] public string ExerciseName { get; set; }
	[FirestoreProperty("videoUrl")] public string VideoUrl { get; set; }
	[FirestoreProperty("focuses")] public IList<string> Focuses { get; set; }
}