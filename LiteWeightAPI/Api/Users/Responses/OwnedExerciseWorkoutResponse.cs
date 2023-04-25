namespace LiteWeightAPI.Api.Users.Responses;

public class OwnedExerciseWorkoutResponse
{
	/// <summary>
	/// Id of the workout.
	/// </summary>
	/// <example>b7c3c321-d5a0-4448-978c-0a8b7709ceb2</example>
	public string WorkoutId { get; set; }

	/// <summary>
	/// Name of the workout.
	/// </summary>
	/// <example>Main Workout</example>
	public string WorkoutName { get; set; }
}