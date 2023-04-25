﻿using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.Users.Requests;

public class SetCurrentWorkoutRequest
{
	/// <summary>
	/// Id of the workout to set as the current workout.
	/// </summary>
	/// <example>4d17e3e7-edf6-41c0-ade2-fdd624b5c9bb</example>
	[Required]
	public string WorkoutId { get; set; }
}