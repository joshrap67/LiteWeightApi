﻿using LiteWeightAPI.Api.SharedWorkouts.Requests;
using LiteWeightAPI.Api.SharedWorkouts.Responses;
using LiteWeightAPI.Commands;
using LiteWeightAPI.Commands.SharedWorkouts.AcceptSharedWorkout;
using LiteWeightAPI.Commands.SharedWorkouts.DeclineWorkout;
using LiteWeightAPI.Commands.SharedWorkouts.GetSharedWorkout;
using LiteWeightAPI.Errors.Attributes;
using LiteWeightAPI.Errors.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LiteWeightAPI.Api.SharedWorkouts;

[Route("shared-workouts")]
[ApiController]
public class SharedWorkoutsController : BaseController
{
	private readonly ICommandDispatcher _commandDispatcher;

	public SharedWorkoutsController(Serilog.ILogger logger, ICommandDispatcher commandDispatcher) : base(logger)
	{
		_commandDispatcher = commandDispatcher;
	}

	/// <summary>Get Shared Workout</summary>
	/// <remarks>Returns a shared workout, assuming it was sent to the authenticated user.</remarks>
	[HttpGet("{sharedWorkoutId}")]
	[ProducesResponseType(typeof(SharedWorkoutResponse), 200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<SharedWorkoutResponse>> GetSharedWorkout(string sharedWorkoutId)
	{
		var sharedWorkout = await _commandDispatcher.DispatchAsync<GetSharedWorkout, SharedWorkoutResponse>(
			new GetSharedWorkout
			{
				UserId = CurrentUserId,
				SharedWorkoutId = sharedWorkoutId
			});
		return sharedWorkout;
	}

	/// <summary>Accept Shared Workout</summary>
	/// <remarks>
	/// Accepts a shared workout and adds any exercises that the user doesn't already own to their owned exercises.<br/><br/>
	/// Accepting a workout deletes the shared workout from the database and creates a workout with the values of that shared workout.
	/// </remarks>
	/// <param name="sharedWorkoutId">Id of the shared workout to accept</param>
	/// <param name="request">Request</param>
	[HttpPost("{sharedWorkoutId}/accept")]
	[InvalidRequest, MaxLimit, AlreadyExists]
	[ProducesResponseType(typeof(AcceptSharedWorkoutResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<AcceptSharedWorkoutResponse>> AcceptSharedWorkout(string sharedWorkoutId,
		AcceptSharedWorkoutRequest request)
	{
		var response = await _commandDispatcher.DispatchAsync<AcceptSharedWorkout, AcceptSharedWorkoutResponse>(
			new AcceptSharedWorkout
			{
				UserId = CurrentUserId,
				SharedWorkoutId = sharedWorkoutId,
				NewName = request.NewName
			});
		return response;
	}

	/// <summary>Decline Shared Workout</summary>
	/// <remarks>Declines a workout and deletes it from the database, assuming the recipient matches the authenticated user.</remarks>
	/// <param name="sharedWorkoutId">Id of the shared workout to decline</param>
	[HttpDelete("{sharedWorkoutId}/decline")]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> DeclineReceivedWorkout(string sharedWorkoutId)
	{
		await _commandDispatcher.DispatchAsync<DeclineWorkout, bool>(new DeclineWorkout
		{
			UserId = CurrentUserId, SharedWorkoutId = sharedWorkoutId
		});
		return Ok();
	}
}