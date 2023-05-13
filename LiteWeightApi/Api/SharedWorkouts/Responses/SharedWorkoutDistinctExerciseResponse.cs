namespace LiteWeightApi.Api.SharedWorkouts.Responses;

public class SharedWorkoutDistinctExerciseResponse
{
	/// <summary>
	/// Name of the exercise.
	/// </summary>
	/// <example>Squat</example>
	public string ExerciseName { get; set; }

	/// <summary>
	/// Video url of the exercise.
	/// </summary>
	/// <example>https://www.youtube.com/watch?v=Dy28eq2PjcM</example>
	public string VideoUrl { get; set; }

	/// <summary>
	/// List of focuses of the exercise.
	/// </summary>
	/// <example>["Legs", "Strength Training"]</example>
	public IList<string> Focuses { get; set; } = new List<string>();
}