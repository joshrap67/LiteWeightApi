namespace LiteWeightAPI.Api.SharedWorkouts.Responses;

public class SharedExerciseResponse
{
	/// <summary>
	/// Name of the exercise
	/// </summary>
	/// <example>Squat</example>
	public string ExerciseName { get; set; }

	/// <summary>
	/// Weight of the exercise.
	/// </summary>
	/// <example>215.0</example>
	public double Weight { get; set; }

	/// <summary>
	/// Number of sets for the exercise.
	/// </summary>
	/// <example>3</example>
	public int Sets { get; set; }

	/// <summary>
	/// Number of reps for the exercise.
	/// </summary>
	/// <example>15</example>
	public int Reps { get; set; }

	/// <summary>
	/// Details of the exercise.
	/// </summary>
	/// <example>Don't lock knees.</example>
	public string Details { get; set; }
}