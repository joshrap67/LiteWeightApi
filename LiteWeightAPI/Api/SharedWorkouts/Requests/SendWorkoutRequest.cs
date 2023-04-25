using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.SharedWorkouts.Requests;

public class SendWorkoutRequest
{
	/// <summary>
	/// Username of the recipient.
	/// </summary>
	/// <example>dennis_r</example>
	[Required]
	public string RecipientUsername { get; set; }

	/// <summary>
	/// Id of the workout to send.
	/// </summary>
	/// <example>718e6712-744a-4075-897e-185d8c455c6a</example>
	[Required]
	public string WorkoutId { get; set; }
}