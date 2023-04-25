using System.ComponentModel.DataAnnotations;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Api.Workouts.Responses;

public class WorkoutResponse
{
	/// <summary>
	/// Id of the workout.
	/// </summary>
	/// <example>b7c3c321-d5a0-4448-978c-0a8b7709ceb2</example>
	public string WorkoutId { get; set; }

	/// <summary>
	/// Name of the workout.
	/// </summary>
	/// <example>Main Workout</example>
	public string WorkoutName { get; set; }

	/// <summary>
	/// Timestamp of when the workout was created (Zulu).
	/// </summary>
	/// <example>2023-04-23T16:49:02.310661Z</example>
	public string CreationDate { get; set; }

	/// <summary>
	/// Username of the user who created the workout.
	/// </summary>
	/// <example>barbell_bill</example>
	public string Creator { get; set; }

	/// <summary>
	/// Routine of the user.
	/// </summary>
	public RoutineResponse Routine { get; set; }

	/// <summary>
	/// Index of the current day of the current week the user is on.
	/// </summary>
	/// <example>0</example>
	[Range(0, Globals.MaxDaysRoutine)]
	public int CurrentDay { get; set; }

	/// <summary>
	/// Index of the current week the user is on.
	/// </summary>
	/// <example>2</example>
	[Range(0, Globals.MaxWeeksRoutine)]
	public int CurrentWeek { get; set; }
}