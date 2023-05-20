using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.Workouts.Requests;

public class CopyWorkoutRequest
{
	/// <summary>
	/// Name for the workout created from the copy.
	/// </summary>
	/// <example>After-Work Workout</example>
	[Required]
	public string NewName { get; set; }
}