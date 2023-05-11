namespace LiteWeightAPI.Api.SharedWorkouts.Responses;

public class SharedWorkoutResponse
{
	/// <summary>
	/// Id of the shared workout.
	/// </summary>
	/// <example>3ac84a61-4822-4ba3-ac93-626fdf087acf</example>
	public string Id { get; set; }

	/// <summary>
	/// Name of the shared workout.
	/// </summary>
	/// <example>Legs Galore</example>
	public string WorkoutName { get; set; }

	/// <summary>
	/// Id of the user who shared the workout.
	/// </summary>
	/// <example>juo06et3-n81k-9bb1-61dj-j12k1152hae1</example>
	public string SenderId { get; set; }

	/// <summary>
	/// Username of the user who shared the workout.
	/// </summary>
	/// <example>arthur_v</example>
	public string SenderUsername { get; set; }

	/// <summary>
	/// Id of the recipient of the shared workout.
	/// </summary>
	/// <example>f1e03cd1-e62c-4a53-84ed-498c72776fc2</example>
	public string RecipientId { get; set; }

	/// <summary>
	/// Routine of the shared workout.
	/// </summary>
	public SharedRoutineResponse Routine { get; set; }

	/// <summary>
	/// List of distinct exercises in the routine of the workout.
	/// </summary>
	public IList<SharedWorkoutExerciseResponse> Exercises { get; set; } = new List<SharedWorkoutExerciseResponse>();
}