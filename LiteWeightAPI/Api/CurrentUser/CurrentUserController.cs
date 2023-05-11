using LiteWeightAPI.Api.Common.Responses.ErrorResponses;
using LiteWeightAPI.Api.CurrentUser.Requests;
using LiteWeightAPI.Api.CurrentUser.Responses;
using LiteWeightAPI.Errors.ErrorAttributes;
using LiteWeightAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiteWeightAPI.Api.CurrentUser;

[Route("current-user")]
[ApiController]
public class CurrentUserController : BaseController
{
	private readonly ICurrentUserService _currentUserService;

	public CurrentUserController(ICurrentUserService currentUserService, Serilog.ILogger logger) : base(logger)
	{
		_currentUserService = currentUserService;
	}

	/// <summary>Get User</summary>
	/// <remarks>Returns the user that is currently authenticated.</remarks>
	[HttpGet]
	[ProducesResponseType(typeof(UserResponse), 200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<UserResponse>> GetUser()
	{
		var user = await _currentUserService.GetUser(CurrentUserId);
		return user;
	}

	/// <summary>Create User</summary>
	/// <remarks>
	/// Creates a user in the database using the email/firebase id in the authenticated token.
	/// <br/>Note that a single verified, authenticated user can only have one user in the database - this is determined by the firebase UUID in the authenticated token.
	/// </remarks>
	[HttpPost]
	[AlreadyExists, InvalidRequest]
	[ProducesResponseType(typeof(UserResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
	{
		// todo is frombody necessary?
		var user = await _currentUserService.CreateUser(request, CurrentUserId);
		return user;
	}

	/// <summary>Update Icon</summary>
	/// <remarks>Updates the user's icon. Note that it just replaces the old icon using the same image url.</remarks>
	[HttpPut("update-icon")]
	[InvalidRequest]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult> UpdateIcon(UpdateIconRequest updateIconRequest)
	{
		await _currentUserService.UpdateIcon(updateIconRequest, CurrentUserId);
		return Ok();
	}

	/// <summary>Set Push Endpoint</summary>
	/// <remarks>Sets the push notification endpoint for the authenticated user.</remarks>
	[HttpPut("set-push-endpoint")]
	[InvalidRequest]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult> SetPushEndpoint(SetPushEndpointRequest request)
	{
		await _currentUserService.SetPushEndpoint(request.FirebaseToken, CurrentUserId);
		return Ok();
	}

	/// <summary>Remove Push Endpoint</summary>
	/// <remarks>Removes the push notification endpoint for the authenticated user. This removes the authenticated user's ability to receive push notifications.</remarks>
	[HttpDelete("remove-push-endpoint")]
	public async Task<ActionResult> RemovePushEndpoint()
	{
		await _currentUserService.DeletePushEndpoint(CurrentUserId);
		return Ok();
	}

	/// <summary>Set All Friend Requests Seen</summary>
	/// <remarks>Sets all friend requests on the authenticated user seen.</remarks>
	[HttpPut("set-all-friend-requests-seen")]
	public async Task<ActionResult> SetAllFriendRequestsSeen()
	{
		await _currentUserService.SetAllFriendRequestsSeen(CurrentUserId);
		return Ok();
	}

	/// <summary>Set Preferences</summary>
	/// <remarks>Sets the preferences on the authenticated user.</remarks>
	[HttpPut("set-user-preferences")]
	[InvalidRequest]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult> SetUserPreferences(UserPreferencesResponse request)
	{
		await _currentUserService.SetUserPreferences(request, CurrentUserId);
		return Ok();
	}

	/// <summary>Set Current Workout</summary>
	/// <remarks>Sets the current workout of the authenticated user to the specified workout.</remarks>
	[HttpPut("set-current-workout")]
	public async Task<ActionResult> SetCurrentWorkout(SetCurrentWorkoutRequest request)
	{
		await _currentUserService.SetCurrentWorkout(request.WorkoutId, CurrentUserId);
		return Ok();
	}

	/// <summary>Set All Received Workouts Seen</summary>
	/// <remarks>Sets all received workouts on the authenticated user to seen.</remarks>
	[HttpPut("received-workouts/set-all-seen")]
	public async Task<ActionResult> SetAllReceivedWorkoutsSeen()
	{
		await _currentUserService.SetAllReceivedWorkoutsSeen(CurrentUserId);
		return Ok();
	}

	/// <summary>Set Received Workout Seen</summary>
	/// <remarks>Sets a given received workout on the authenticated user to seen.</remarks>
	[HttpPut("received-workouts/{workoutId}/set-seen")]
	public async Task<ActionResult> SetReceivedWorkoutSeen(string workoutId)
	{
		await _currentUserService.SetReceivedWorkoutSeen(workoutId, CurrentUserId);
		return Ok();
	}

	/// <summary>Delete Current User</summary>
	/// <remarks>
	/// Deletes a user and all data associated with it.<br/>
	/// Data deleted: user, workouts belonging to user, shared workouts sent to user, images belonging to user.</remarks>
	[HttpDelete]
	public async Task<ActionResult> DeleteUser()
	{
		await _currentUserService.DeleteUser(CurrentUserId);
		return Ok();
	}
}