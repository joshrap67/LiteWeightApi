using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.Workouts.Requests;

public class CopyWorkoutRequest
{
	/// <summary>
	/// Id of the workout to copy.
	/// </summary>
	/// <example>0a24361e-209e-49d8-82a0-67938e1ee2c7</example>
	[Required]
	public string WorkoutId { get; set; }

	/// <summary>
	/// Name for the workout created from the copy.
	/// </summary>
	/// <example>After-Work Workout</example>
	[Required]
	public string NewName { get; set; }
}