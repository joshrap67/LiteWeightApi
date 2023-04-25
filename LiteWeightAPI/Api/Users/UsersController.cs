using LiteWeightAPI.Api.Common.Responses.ErrorResponses;
using LiteWeightAPI.Api.Users.Requests;
using LiteWeightAPI.Api.Users.Responses;
using LiteWeightAPI.Errors.ErrorAttributes;
using LiteWeightAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiteWeightAPI.Api.Users;

[Route("users")]
[ApiController]
public class UsersController : BaseController
{
	private readonly IUserService _userService;

	public UsersController(IUserService userService, Serilog.ILogger logger) : base(logger)
	{
		_userService = userService;
	}

	/// <summary>Get User</summary>
	/// <remarks>Returns the user that is currently authenticated.</remarks>
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<UserResponse>), 200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)] // todo throw this
	public async Task<ActionResult<UserResponse>> GetUser()
	{
		var user = await _userService.GetUser(UserId);
		return user;
	}

	/// <summary>Create User</summary>
	/// <remarks>Creates a user using the username in the authenticated token.</remarks>
	[HttpPost]
	[InvalidRequest]
	[ProducesResponseType(typeof(UserResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult<UserResponse>> CreateUser()
	{
		var user = await _userService.CreateUser(UserId);
		return user;
	}

	/// <summary>Update Icon</summary>
	/// <remarks>Updates the user's icon. Note that it just replaces the old icon using the same image url.</remarks>
	[HttpPut("update-icon")]
	public async Task<ActionResult> UpdateIcon(UpdateIconRequest updateIconRequest)
	{
		await _userService.UpdateIcon(updateIconRequest, UserId);
		return Ok();
	}

	/// <summary>Set Push Endpoint</summary>
	/// <remarks>Sets the push notification endpoint for the authenticated user.</remarks>
	[HttpPut("set-push-endpoint")]
	public async Task<ActionResult> SetPushEndpoint(SetPushEndpointRequest request)
	{
		await _userService.SetPushEndpoint(request.FirebaseToken, UserId);
		return Ok();
	}

	/// <summary>Remove Push Endpoint</summary>
	/// <remarks>Removes the push notification endpoint for the authenticated user. This removes the authenticated user's ability to receive push notifications.</remarks>
	[HttpDelete("remove-push-endpoint")]
	public async Task<ActionResult> RemovePushEndpoint()
	{
		await _userService.DeletePushEndpoint(UserId);
		return Ok();
	}

	/// <summary>Send Friend Request</summary>
	/// <remarks>Sends a friend request to the specified user, assuming valid input</remarks>
	[HttpPut("send-friend-request")]
	public async Task<ActionResult<FriendResponse>> SendFriendRequest(RecipientRequest request)
	{
		var response = await _userService.SendFriendRequest(request.Username, UserId);
		return response;
	}

	/// <summary>Accept Friend Request</summary>
	/// <remarks>Accepts a friend request from the specified user.</remarks>
	[HttpPut("accept-friend-request")]
	public async Task<ActionResult> AcceptFriendRequest(RecipientRequest request)
	{
		await _userService.AcceptFriendRequest(request.Username, UserId);
		return Ok();
	}

	/// <summary>Cancel Friend Request</summary>
	/// <remarks>Cancels a friend request sent to the specified user.</remarks>
	[HttpPut("cancel-friend-request")]
	public async Task<ActionResult> CancelFriendRequest(RecipientRequest request)
	{
		await _userService.CancelFriendRequest(request.Username, UserId);
		return Ok();
	}

	/// <summary>Decline Friend Request</summary>
	/// <remarks>Declines a friend request from the specified user.</remarks>
	[HttpPut("decline-friend-request")]
	public async Task<ActionResult> DeclineFriendRequest(RecipientRequest request)
	{
		await _userService.DeclineFriendRequest(request.Username, UserId);
		return Ok();
	}

	/// <summary>Remove Friend</summary>
	/// <remarks>Removes the specified user as a friend.</remarks>
	[HttpDelete("remove-friend")]
	public async Task<ActionResult> RemoveFriend(RecipientRequest request)
	{
		await _userService.RemoveFriend(request.Username, UserId);
		return Ok();
	}

	/// <summary>Block User</summary>
	/// <remarks>Blocks the specified user.</remarks>
	[HttpPut("block-user")]
	public async Task<ActionResult<BlockedUserResponse>> BlockUser(RecipientRequest request)
	{
		var response = await _userService.BlockUser(request.Username, UserId);
		return response;
	}

	/// <summary>Set All Friend Requests Seen</summary>
	/// <remarks>Sets all friend requests on the authenticated user seen.</remarks>
	[HttpPut("set-all-friend-requests-seen")]
	public async Task<ActionResult> SetAllFriendRequestsSeen()
	{
		await _userService.SetAllFriendRequestsSeen(UserId);
		return Ok();
	}

	/// <summary>Set Preferences</summary>
	/// <remarks>Sets the preferences on the authenticated user.</remarks>
	[HttpPut("set-user-preferences")]
	public async Task<ActionResult> SetUserPreferences(UserPreferencesResponse request)
	{
		await _userService.SetUserPreferences(request, UserId);
		return Ok();
	}

	/// <summary>Create Exercise</summary>
	/// <remarks>Creates an exercise owned by the authenticated user. Note that duplicates (by name) are not allowed.</remarks>
	[HttpPost("create-exercise")]
	[ProducesResponseType(typeof(OwnedExerciseWorkoutResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult<OwnedExerciseResponse>> CreateExercise(CreateExerciseRequest request)
	{
		var response = await _userService.CreateExercise(request, UserId);
		return response;
	}

	/// <summary>Update Exercise</summary>
	/// <remarks>Updates an exercise owned by the authenticated user, if it exists. Note that duplicates (by name) are not allowed.</remarks>
	[HttpPut("update-exercise")]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult> UpdateExercise(UpdateExerciseRequest request)
	{
		await _userService.UpdateExercise(request, UserId);
		return Ok();
	}

	/// <summary>Delete Exercise</summary>
	/// <remarks>Removes an exercise owned by the authenticated user. Doing so removes it from any workout.</remarks>
	[HttpDelete("delete-exercise/{exerciseId}")]
	public async Task<ActionResult> DeleteExercise(string exerciseId)
	{
		await _userService.DeleteExercise(exerciseId, UserId);
		return Ok();
	}

	/// <summary>Reset Workout Statistics</summary>
	/// <remarks>Resets the statistics for a given workout, if it exists.</remarks>
	[HttpPut("{workoutId}/reset-statistics")]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult<UserResponse>> ResetStatistics(string workoutId)
	{
		var response = await _userService.RestartStatistics(workoutId, UserId);
		return response;
	}

	/// <summary>Set Current Workout</summary>
	/// <remarks>Sets the current workout of the authenticated user to the specified workout.</remarks>
	[HttpPut("set-current-workout")]
	public async Task<ActionResult> SetCurrentWorkout(SetCurrentWorkoutRequest request)
	{
		await _userService.SetCurrentWorkout(request.WorkoutId, UserId);
		return Ok();
	}

	/// <summary>Set All Received Workouts Seen</summary>
	/// <remarks>Sets all received workouts on the authenticated user to seen.</remarks>
	[HttpPut("received-workouts/set-all-seen")]
	public async Task<ActionResult> SetAllReceivedWorkoutsSeen()
	{
		await _userService.SetAllReceivedWorkoutsSeen(UserId);
		return Ok();
	}

	/// <summary>Set Received Workout Seen</summary>
	/// <remarks>Sets a given received workout on the authenticated user to seen.</remarks>
	[HttpPut("received-workouts/{workoutId}/set-seen")]
	public async Task<ActionResult> SetReceivedWorkoutSeen(string workoutId)
	{
		await _userService.SetReceivedWorkoutSeen(workoutId, UserId);
		return Ok();
	}

	/// <summary>Delete User</summary>
	/// <remarks>
	/// Deletes a user and all data associated with it.<br/>
	/// Data deleted: user, workouts belonging to user, shared workouts sent to user, images belonging to user.</remarks>
	[HttpDelete]
	public async Task<ActionResult> DeleteUser()
	{
		await _userService.DeleteUser(UserId);
		return Ok();
	}
	// todo feedback controller?
}