namespace LiteWeightAPI.Api.SharedWorkouts.Responses;

public class SharedWorkoutResponse
{
	/// <summary>
	/// Id of the shared workout.
	/// </summary>
	/// <example>3ac84a61-4822-4ba3-ac93-626fdf087acf</example>
	public string SharedWorkoutId { get; set; }

	/// <summary>
	/// Name of the shared workout.
	/// </summary>
	/// <example>Legs Galore</example>
	public string WorkoutName { get; set; }

	/// <summary>
	/// Username of the user who created the workout.
	/// </summary>
	/// <example>arthur_v</example>
	public string Creator { get; set; }

	/// <summary>
	/// Routine of the shared workout.
	/// </summary>
	public SharedRoutineResponse Routine { get; set; }

	/// <summary>
	/// List of distinct exercises in the routine of the workout.
	/// </summary>
	public IList<SharedWorkoutExerciseResponse> Exercises { get; set; } = new List<SharedWorkoutExerciseResponse>();
}