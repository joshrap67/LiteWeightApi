namespace LiteWeightAPI.Api.SharedWorkouts.Requests;

public class AcceptSharedWorkoutRequest
{
	/// <summary>
	/// Optional name to set for the workout once accepted. Must be unique.
	/// </summary>
	/// <example>Olympic Routine</example>
	public string NewName { get; set; }
}