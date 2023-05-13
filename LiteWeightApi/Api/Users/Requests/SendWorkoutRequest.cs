using System.ComponentModel.DataAnnotations;

namespace LiteWeightApi.Api.Users.Requests;

public class SendWorkoutRequest
{
	/// <summary>
	/// Id of the workout to send.
	/// </summary>
	/// <example>718e6712-744a-4075-897e-185d8c455c6a</example>
	[Required]
	public string WorkoutId { get; set; }

	/// <summary>
	/// Username to send the workout to. Must be a valid user.
	/// </summary>
	[Required]
	public string RecipientUsername { get; set; }
}