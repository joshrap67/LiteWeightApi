using System.ComponentModel.DataAnnotations;

namespace LiteWeightApi.Api.Workouts.Requests;

public class RestartWorkoutRequest
{
	/// <summary>
	/// Workout to restart.
	/// </summary>
	[Required]
	public SetRoutineRequest Routine { get; set; }
}