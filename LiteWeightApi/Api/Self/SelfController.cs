using LiteWeightAPI.Api.Self.Requests;
using LiteWeightAPI.Api.Self.Responses;
using LiteWeightAPI.Errors.Attributes;
using LiteWeightAPI.Errors.Responses;
using LiteWeightAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiteWeightAPI.Api.Self;

[Route("self")]
[ApiController]
public class SelfController : BaseController
{
	private readonly ISelfService _selfService;

	public SelfController(ISelfService selfService, Serilog.ILogger logger) : base(logger)
	{
		_selfService = selfService;
	}

	/// <summary>Get Self</summary>
	/// <remarks>Returns the user that is currently authenticated.</remarks>
	[HttpGet]
	[ProducesResponseType(typeof(UserResponse), 200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<UserResponse>> GetSelf()
	{
		var user = await _selfService.GetSelf(CurrentUserId);
		return user;
	}

	/// <summary>Create Self</summary>
	/// <remarks>
	/// Creates a user in the database using the email/firebase id in the authenticated token.
	/// <br/>Note that a single verified, authenticated user can only have one user in the database - this is determined by the firebase UUID in the authenticated token.
	/// </remarks>
	[HttpPost]
	[AlreadyExists, InvalidRequest]
	[ProducesResponseType(typeof(UserResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult<UserResponse>> CreateSelf(CreateUserRequest request)
	{
		var user = await _selfService.CreateSelf(request, CurrentUserEmail, CurrentUserId);
		return user;
	}

	/// <summary>Update Profile Picture</summary>
	/// <remarks>Updates the user's profile picture. Note that it just replaces the old picture using the same image url.</remarks>
	[HttpPut("profile-picture")]
	[InvalidRequest]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult> UpdateProfilePicture(UpdateProfilePictureRequest updateProfilePictureRequest)
	{
		await _selfService.UpdateProfilePicture(updateProfilePictureRequest, CurrentUserId);
		return Ok();
	}

	/// <summary>Link Firebase Token</summary>
	/// <remarks>Links the firebase token to the authenticated user. This enables the authenticated user's ability to receive push notifications.</remarks>
	[HttpPut("link-firebase-token")]
	[InvalidRequest]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult> LinkFirebaseToken(LinkFirebaseTokenRequest request)
	{
		await _selfService.LinkFirebaseToken(request.FirebaseToken, CurrentUserId);
		return Ok();
	}

	/// <summary>Unset Firebase Token</summary>
	/// <remarks>Unlinks the firebase token associated for the authenticated user. This removes the authenticated user's ability to receive push notifications.</remarks>
	[HttpDelete("unlink-firebase-token")]
	public async Task<ActionResult> UnlinkFirebaseToken()
	{
		await _selfService.UnlinkFirebaseToken(CurrentUserId);
		return Ok();
	}

	/// <summary>Set All Friend Requests Seen</summary>
	/// <remarks>Sets all friend requests on the authenticated user seen.</remarks>
	[HttpPut("all-friend-requests-seen")]
	public async Task<ActionResult> SetAllFriendRequestsSeen()
	{
		await _selfService.SetAllFriendRequestsSeen(CurrentUserId);
		return Ok();
	}

	/// <summary>Set Preferences</summary>
	/// <remarks>Sets the preferences on the authenticated user.</remarks>
	[HttpPut("user-preferences")]
	[InvalidRequest]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult> SetUserPreferences(UserPreferencesResponse request)
	{
		await _selfService.SetUserPreferences(request, CurrentUserId);
		return Ok();
	}

	/// <summary>Set Current Workout</summary>
	/// <remarks>Sets the current workout of the authenticated user to the specified workout, if it exists.</remarks>
	[HttpPut("current-workout")]
	public async Task<ActionResult> SetCurrentWorkout(SetCurrentWorkoutRequest request)
	{
		await _selfService.SetCurrentWorkout(request.WorkoutId, CurrentUserId);
		return Ok();
	}

	/// <summary>Set All Received Workouts Seen</summary>
	/// <remarks>Sets all received workouts on the authenticated user to seen.</remarks>
	[HttpPut("received-workouts/all-seen")]
	public async Task<ActionResult> SetAllReceivedWorkoutsSeen()
	{
		await _selfService.SetAllReceivedWorkoutsSeen(CurrentUserId);
		return Ok();
	}

	/// <summary>Set Received Workout Seen</summary>
	/// <remarks>Sets a given received workout on the authenticated user to seen.</remarks>
	[HttpPut("received-workouts/{workoutId}/seen")]
	public async Task<ActionResult> SetReceivedWorkoutSeen(string workoutId)
	{
		await _selfService.SetReceivedWorkoutSeen(workoutId, CurrentUserId);
		return Ok();
	}

	/// <summary>Delete Current User</summary>
	/// <remarks>
	/// Deletes a user and all data associated with it.<br/>
	/// Data deleted: user, workouts belonging to user, shared workouts sent to user, images belonging to user.</remarks>
	[HttpDelete]
	public async Task<ActionResult> DeleteUser()
	{
		await _selfService.DeleteUser(CurrentUserId);
		return Ok();
	}
}