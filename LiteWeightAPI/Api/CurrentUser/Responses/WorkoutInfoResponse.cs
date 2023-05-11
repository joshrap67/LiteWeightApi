namespace LiteWeightAPI.Api.CurrentUser.Responses;

public class WorkoutInfoResponse
{
	/// <summary>
	/// Id of the workout.
	/// </summary>
	/// <example>84a34611-9f4c-443f-97f9-cbf5cde69c65</example>
	public string WorkoutId { get; set; }

	/// <summary>
	/// Name of the workout.
	/// </summary>
	/// <example>3-Day Split</example>
	public string WorkoutName { get; set; }

	/// <summary>
	/// Timestamp of when the workout was last modified.
	/// </summary>
	/// <example>2023-04-06T23:20:39.665047Z</example>
	public string LastModified { get; set; } // todo need to make this more accurate

	/// <summary>
	/// Total times the workout has been completed (restarted).
	/// </summary>
	/// <example>15</example>
	public int TimesCompleted { get; set; }

	/// <summary>
	/// Average of all exercises completed on the workout.
	/// </summary>
	/// <example>94.2</example>
	public double AverageExercisesCompleted { get; set; }
}