using LiteWeightAPI.Api.Common.Responses;
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

	/// <summary>Share Workout</summary>
	/// <remarks>Create a shared workout from a specified workout, and send it to a specified user.</remarks>
	[HttpPost]
	[InvalidRequest, UserNotFound, MaximumReached, MiscError]
	[ProducesResponseType(typeof(ResourceCreatedResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult<ResourceCreatedResponse>> ShareWorkout(SendWorkoutRequest request)
	{
		var createdWorkoutId = await _sharedWorkoutService.ShareWorkout(request, UserId);
		return new ResourceCreatedResponse { Id = createdWorkoutId };
	}

	/// <summary>Get Shared Workout</summary>
	/// <remarks>Returns a given shared workout assuming it was sent to the authenticated user.</remarks>
	[HttpGet("{sharedWorkoutId}")]
	[ProducesResponseType(typeof(SharedWorkoutResponse), 200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<SharedWorkoutResponse>> GetSharedWorkout(string sharedWorkoutId)
	{
		var sharedWorkout = await _sharedWorkoutService.GetSharedWorkout(sharedWorkoutId);
		return sharedWorkout;
	}

	/// <summary>Accept Shared Workout</summary>
	/// <remarks>
	/// Accepts a shared workout and adds any exercises that the user doesn't already own to their owned exercises. Accepting a
	/// workout deletes the shared workout from the database and creates a workout with the values of that shared workout.
	/// </remarks>
	/// <param name="sharedWorkoutId">Id of the shared workout to accept</param>
	/// <param name="newName">Optional parameter that specifies a different name to accept the workout as</param>
	[HttpPost("{sharedWorkoutId}/accept")]
	[MaximumReached, UserNotFound, DuplicateFound]
	[ProducesResponseType(typeof(AcceptSharedWorkoutResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<AcceptSharedWorkoutResponse>> AcceptSharedWorkout(string sharedWorkoutId,
		[FromQuery] string newName)
	{
		var response = await _sharedWorkoutService.AcceptWorkout(sharedWorkoutId, UserId, newName);
		return response;
	}

	/// <summary>Decline Shared Workout</summary>
	/// <remarks>Declines a workout and deletes it from the database.</remarks>
	[HttpPost("{sharedWorkoutId}/decline")]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> DeclineReceivedWorkout(string sharedWorkoutId)
	{
		await _sharedWorkoutService.DeclineWorkout(sharedWorkoutId, UserId);
		return Ok();
	}
}