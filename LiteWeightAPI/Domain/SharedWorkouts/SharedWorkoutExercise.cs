using LiteWeightAPI.Domain.Users;

namespace LiteWeightAPI.Domain.SharedWorkouts;

public class SharedWorkoutExercise
{
	public SharedWorkoutExercise(OwnedExercise userExercise, string exerciseName)
	{
		ExerciseName = exerciseName;
		VideoUrl = userExercise.VideoUrl;
		Focuses = userExercise.Focuses;
	}

	public string ExerciseName { get; set; }
	public string VideoUrl { get; set; }
	public IList<string> Focuses { get; set; }
}