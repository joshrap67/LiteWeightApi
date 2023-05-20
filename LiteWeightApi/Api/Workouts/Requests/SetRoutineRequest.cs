﻿using System.ComponentModel.DataAnnotations;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Api.Workouts.Requests;

public class SetRoutineRequest
{
	/// <summary>
	/// Weeks of the routine
	/// </summary>
	[Required]
	public IList<SetRoutineWeekRequest> Weeks { get; set; } = new List<SetRoutineWeekRequest>();
}

public class SetRoutineWeekRequest
{
	/// <summary>
	/// Days of the routine
	/// </summary>
	[Required]
	public IList<SetRoutineDayRequest> Days { get; set; } = new List<SetRoutineDayRequest>();
}

public class SetRoutineDayRequest
{
	/// <summary>
	/// Arbitrary tag of the day.
	/// </summary>
	/// <example>Back and Biceps Day</example>
	[MaxLength(Globals.MaxDayTagLength)]
	public string Tag { get; set; }

	/// <summary>
	/// List of exercises for the given day.
	/// </summary>
	[Required]
	public IList<SetRoutineExerciseRequest> Exercises { get; set; } = new List<SetRoutineExerciseRequest>();
}

public class SetRoutineExerciseRequest
{
	/// <summary>
	/// Id of the exercise (reference to the list of exercises on the user).
	/// </summary>
	/// <example>88a54457-2253-404e-ac09-82a8f2ce5fb8</example>
	[Required]
	public string ExerciseId { get; set; }

	/// <summary>
	/// Has the user completed this exercise?
	/// </summary>
	public bool Completed { get; set; }

	/// <summary>
	/// Weight of the exercise.
	/// </summary>
	/// <example>30.0</example>
	[Required]
	[Range(0.0, Globals.MaxWeight)]
	public double Weight { get; set; }

	/// <summary>
	/// Number of sets for the exercise.
	/// </summary>
	/// <example>3</example>
	[Required]
	[Range(0, Globals.MaxSets)]
	public int Sets { get; set; }

	/// <summary>
	/// Number of reps for the exercise.
	/// </summary>
	/// <example>15</example>
	[Required]
	[Range(0, Globals.MaxReps)]
	public int Reps { get; set; }

	/// <summary>
	/// Details of the exercise.
	/// </summary>
	/// <example>Don't overextend arms.</example>
	[MaxLength(Globals.MaxDetailsLength)]
	public string Details { get; set; }
}