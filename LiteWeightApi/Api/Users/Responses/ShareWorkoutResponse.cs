namespace LiteWeightApi.Api.Users.Responses;

public class ShareWorkoutResponse
{
	/// <summary>
	/// Id of the shared workout that was created as a result of sending the workout.
	/// </summary>
	/// <example>3ac84a61-4822-4ba3-ac93-626fdf087acf</example>
	public string SharedWorkoutId { get; set; }
}