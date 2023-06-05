using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.Workouts.Requests;

public class RenameWorkoutRequest
{
	/// <summary>
	/// New name of the workout. Must be unique.
	/// </summary>
	[Required]
	public string Name { get; set; }
}