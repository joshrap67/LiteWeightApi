using System.ComponentModel.DataAnnotations;
using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Api.Workouts.Requests;

public class SwitchWorkoutRequest
{
	/// <summary>
	/// Id of the workout to switch to.
	/// </summary>
	/// <example>8bc6a04a-879f-4ed0-959b-562908e38be6</example>
	[Required]
	public string NewWorkoutId { get; set; }

	/// <summary>
	/// Optional workout to update when switching workouts.
	/// </summary>
	public WorkoutResponse WorkoutToUpdate { get; set; }
}