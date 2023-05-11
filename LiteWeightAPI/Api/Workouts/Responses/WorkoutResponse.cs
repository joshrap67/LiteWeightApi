namespace LiteWeightAPI.Api.Workouts.Responses;

public class WorkoutResponse
{
	/// <summary>
	/// Unique identifier of the workout.
	/// </summary>
	/// <example>b7c3c321-d5a0-4448-978c-0a8b7709ceb2</example>
	public string Id { get; set; }

	/// <summary>
	/// Name of the workout.
	/// </summary>
	/// <example>Main Workout</example>
	public string Name { get; set; }

	/// <summary>
	/// Timestamp of when the workout was created (Zulu).
	/// </summary>
	/// <example>2023-04-23T16:49:02.310661Z</example>
	public string CreationTimestamp { get; set; }

	/// <summary>
	/// Id of the user who created the workout.
	/// </summary>
	/// <example>a7be7348-bca5-466c-b290-55ae38e2bad0</example> todo guid
	public string CreatorId { get; set; }

	/// <summary>
	/// Routine of the user.
	/// </summary>
	public RoutineResponse Routine { get; set; }

	/// <summary>
	/// Index of the current week the user is on.
	/// </summary>
	/// <example>2</example>
	public int CurrentWeek { get; set; }

	/// <summary>
	/// Index of the current day of the current week the user is on.
	/// </summary>
	/// <example>0</example>
	public int CurrentDay { get; set; }
}