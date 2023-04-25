using System.ComponentModel.DataAnnotations;
using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Api.Workouts.Requests;

public class SetRoutineRequest
{
	/// <summary>
	/// Routine to set.
	/// </summary>
	[Required]
	public RoutineResponse Routine { get; set; }
}