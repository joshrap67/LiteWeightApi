using System.ComponentModel.DataAnnotations;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Api.SharedWorkouts.Requests;

public class AcceptSharedWorkoutRequest
{
	/// <summary>
	/// Optional name to set for the workout once accepted. Must be unique. If not specified, the created workout will have the name of the shared workout.
	/// </summary>
	/// <example>Olympic Routine</example>
	[MaxLength(Globals.MaxWorkoutNameLength)]
	public string WorkoutName { get; set; }
}