using LiteWeightAPI.Api.Common.Responses.ErrorResponses;
using LiteWeightAPI.Api.SharedWorkouts.Requests;
using LiteWeightAPI.Api.SharedWorkouts.Responses;
using LiteWeightAPI.Errors.ErrorAttributes;
using LiteWeightAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiteWeightAPI.Api.SharedWorkouts;

[Route("shared-workouts")]
[ApiController]
public class SharedWorkoutsController : BaseController
{
	private readonly ISharedWorkoutService _sharedWorkoutService;

	public SharedWorkoutsController(Serilog.ILogger logger, ISharedWorkoutService sharedWorkoutService) : base(logger)
	{
		_sharedWorkoutService = sharedWorkoutService;
	}

	/// <summary>Get Shared Workout</summary>
	/// <remarks>Returns a given shared workout, assuming it was sent to the authenticated user.</remarks>
	[HttpGet("{sharedWorkoutId}")]
	[ProducesResponseType(typeof(SharedWorkoutResponse), 200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<SharedWorkoutResponse>> GetSharedWorkout(string sharedWorkoutId)
	{
		var sharedWorkout = await _sharedWorkoutService.GetSharedWorkout(sharedWorkoutId, CurrentUserId);
		return sharedWorkout;
	}

	/// <summary>Accept Shared Workout</summary>
	/// <remarks>
	/// Accepts a shared workout and adds any exercises that the user doesn't already own to their owned exercises. Accepting a
	/// workout deletes the shared workout from the database and creates a workout with the values of that shared workout.
	/// </remarks>
	/// <param name="sharedWorkoutId">Id of the shared workout to accept</param>
	/// <param name="request">Request</param>
	[HttpPost("{sharedWorkoutId}/accept")]
	[MaxLimit, UserNotFound, AlreadyExists]
	[ProducesResponseType(typeof(AcceptSharedWorkoutResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<AcceptSharedWorkoutResponse>> AcceptSharedWorkout(string sharedWorkoutId,
		AcceptSharedWorkoutRequest request)
	{
		// todo can this cause a invalid request? idk since i think .net would try and cast everything to a string
		var response = await _sharedWorkoutService.AcceptWorkout(sharedWorkoutId, CurrentUserId, request);
		return response;
	}

	/// <summary>Decline Shared Workout</summary>
	/// <remarks>Declines a workout and deletes it from the database, assuming the recipient matches the authenticated user.</remarks>
	[HttpPost("{sharedWorkoutId}/decline")]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> DeclineReceivedWorkout(string sharedWorkoutId)
	{
		// todo delete?
		await _sharedWorkoutService.DeclineWorkout(sharedWorkoutId, CurrentUserId);
		return Ok();
	}
}