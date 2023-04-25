namespace LiteWeightAPI.Api.Users.Responses;

public class SharedWorkoutMetaResponse
{
	/// <summary>
	/// Id of the shared workout.
	/// </summary>
	/// <example>194939d5-c40d-43e5-b40a-7d30d764b7f7</example>
	public string WorkoutId { get; set; }

	/// <summary>
	/// Name of the shared workout.
	/// </summary>
	/// <example>High Intensity Workout</example>
	public string WorkoutName { get; set; }

	/// <summary>
	/// Timestamp of when the workout was shared (Zulu)
	/// </summary>
	/// <example>2023-04-23T13:43:44.685341Z</example>
	public string DateSent { get; set; }

	/// <summary>
	/// Is this received workout seen by the user?
	/// </summary>
	public bool Seen { get; set; }

	/// <summary>
	/// User who sent the shared workout.
	/// </summary>
	/// <example>jessica78</example>
	public string Sender { get; set; }

	/// <summary>
	/// File path of the user's icon
	/// </summary>
	/// <example>61fcf9b4-15f1-4413-9534-683b085875b9.jpg</example>
	public string Icon { get; set; }

	/// <summary>
	/// Total number of days in the shared workout.
	/// </summary>
	/// <example>16</example>
	public int TotalDays { get; set; }

	/// <summary>
	/// Most frequent exercise focus of the shared workout.
	/// </summary>
	/// <example>Biceps</example>
	public string MostFrequentFocus { get; set; }
}