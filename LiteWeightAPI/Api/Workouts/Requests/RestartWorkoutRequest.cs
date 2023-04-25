using System.ComponentModel.DataAnnotations;
using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Api.Workouts.Requests;

public class RestartWorkoutRequest
{
	/// <summary>
	/// Workout to restart.
	/// </summary>
	[Required]
	public WorkoutResponse Workout { get; set; }
}