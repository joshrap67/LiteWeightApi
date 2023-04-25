using System.ComponentModel.DataAnnotations;
using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Api.Workouts.Requests;

public class CreateWorkoutRequest
{
	/// <summary>
	/// Name of the workout.
	/// </summary>
	/// <example>High Intensity Workout</example>
	[Required]
	public string WorkoutName { get; set; }

	/// <summary>
	/// Routine of the workout.
	/// </summary>
	[Required]
	public RoutineResponse Routine { get; set; } // todo new model?

	/// <summary>
	/// If true, set this new workout to be the current workout for the user creating the workout.
	/// </summary>
	public bool SetAsCurrentWorkout { get; set; } = true;
}