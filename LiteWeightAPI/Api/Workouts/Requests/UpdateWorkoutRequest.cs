using System.ComponentModel.DataAnnotations;
using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Api.Workouts.Requests;

public class UpdateWorkoutRequest
{
	/// <summary>
	/// Workout to update.
	/// </summary>
	[Required]
	public WorkoutResponse Workout { get; set; }
}