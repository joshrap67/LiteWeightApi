using LiteWeightAPI.Api.SharedWorkouts.Requests;
using LiteWeightAPI.Api.SharedWorkouts.Responses;
using LiteWeightAPI.Commands;
using LiteWeightAPI.Commands.SharedWorkouts.AcceptSharedWorkout;
using LiteWeightAPI.Commands.SharedWorkouts.DeclineSharedWorkout;
using LiteWeightAPI.Commands.SharedWorkouts.GetSharedWorkout;
using LiteWeightAPI.Errors.Attributes;
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
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
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
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
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
		return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
	}

	/// <summary>Decline Shared Workout</summary>
	/// <remarks>Declines a workout and deletes it from the database, assuming the recipient matches the authenticated user.</remarks>
	/// <param name="sharedWorkoutId">Id of the shared workout to decline</param>
	[HttpDelete("{sharedWorkoutId}/decline")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> DeclineSharedWorkout(string sharedWorkoutId)
	{
		await _commandDispatcher.DispatchAsync<DeclineSharedWorkout, bool>(new DeclineSharedWorkout
		{
			UserId = CurrentUserId, SharedWorkoutId = sharedWorkoutId
		});
		return NoContent();
	}
}