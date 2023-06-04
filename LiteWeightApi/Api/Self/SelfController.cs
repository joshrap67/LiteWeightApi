using AutoMapper;
using LiteWeightAPI.Api.Self.Requests;
using LiteWeightAPI.Api.Self.Responses;
using LiteWeightAPI.Commands;
using LiteWeightAPI.Commands.Self.CreateSelf;
using LiteWeightAPI.Commands.Self.DeleteSelf;
using LiteWeightAPI.Commands.Self.GetSelf;
using LiteWeightAPI.Commands.Self.SetAllFriendRequestsSeen;
using LiteWeightAPI.Commands.Self.SetAllReceivedWorkoutsSeen;
using LiteWeightAPI.Commands.Self.SetCurrentWorkout;
using LiteWeightAPI.Commands.Self.SetFirebaseToken;
using LiteWeightAPI.Commands.Self.SetPreferences;
using LiteWeightAPI.Commands.Self.SetReceivedWorkoutSeen;
using LiteWeightAPI.Commands.Self.SetUsername;
using LiteWeightAPI.Commands.Self.UpdateProfilePicture;
using LiteWeightAPI.Errors.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace LiteWeightAPI.Api.Self;

[Route("self")]
[ApiController]
public class SelfController : BaseController
{
	private readonly ICommandDispatcher _commandDispatcher;
	private readonly IMapper _mapper;

	public SelfController(ICommandDispatcher commandDispatcher, IMapper mapper, Serilog.ILogger logger) : base(logger)
	{
		_commandDispatcher = commandDispatcher;
		_mapper = mapper;
	}

	/// <summary>Get Self</summary>
	/// <remarks>Returns the user that is currently authenticated.</remarks>
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<UserResponse>> GetSelf()
	{
		var user = await _commandDispatcher.DispatchAsync<GetSelf, UserResponse>(new GetSelf
			{ UserId = CurrentUserId });
		return user;
	}

	/// <summary>Create Self</summary>
	/// <remarks>
	/// Creates a user in the database using the email/firebase id in the authenticated token.
	/// <br/><br/>Note that a single verified, authenticated user can only have one user in the database - this is determined by the firebase UUID in the authenticated token.
	/// </remarks>
	[HttpPost]
	[AlreadyExists, InvalidRequest]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<UserResponse>> CreateSelf(CreateUserRequest request)
	{
		var command = _mapper.Map<CreateSelf>(request);
		command.UserEmail = CurrentUserEmail;
		command.UserId = CurrentUserId;

		var user = await _commandDispatcher.DispatchAsync<CreateSelf, UserResponse>(command);
		return new ObjectResult(user) { StatusCode = StatusCodes.Status201Created };
	}

	/// <summary>Update Profile Picture</summary>
	/// <remarks>Updates the user's profile picture. Note that it simply replaces the old picture using the same image url.</remarks>
	[HttpPut("profile-picture")]
	[InvalidRequest]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> UpdateProfilePicture(UpdateProfilePictureRequest request)
	{
		await _commandDispatcher.DispatchAsync<UpdateProfilePicture, bool>(new UpdateProfilePicture
		{
			UserId = CurrentUserId, ImageData = request.ImageData
		});
		return NoContent();
	}

	/// <summary>Link Firebase Token</summary>
	/// <remarks>Links the firebase token to the authenticated user. This enables the authenticated user's ability to receive push notifications.</remarks>
	[HttpPut("link-firebase-token")]
	[InvalidRequest]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> LinkFirebaseToken(LinkFirebaseTokenRequest request)
	{
		await _commandDispatcher.DispatchAsync<SetFirebaseToken, bool>(new SetFirebaseToken
		{
			UserId = CurrentUserId, Token = request.FirebaseToken
		});
		return NoContent();
	}

	/// <summary>Unlink Firebase Token</summary>
	/// <remarks>Unlinks the firebase token associated for the authenticated user. This removes the authenticated user's ability to receive push notifications.</remarks>
	[HttpPut("unlink-firebase-token")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> UnlinkFirebaseToken()
	{
		await _commandDispatcher.DispatchAsync<SetFirebaseToken, bool>(new SetFirebaseToken
		{
			UserId = CurrentUserId, Token = null
		});
		return NoContent();
	}

	/// <summary>Set Username</summary>
	/// <remarks>Updates the user of the authenticated user. Also updates any friends of the user or friend requests by the user to have this new username.</remarks>
	[HttpPut("username")]
	[InvalidRequest, AlreadyExists]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> SetUsername(SetUsernameRequest request)
	{
		await _commandDispatcher.DispatchAsync<SetUsername, bool>(new SetUsername
		{
			UserId = CurrentUserId, NewUsername = request.NewUsername
		});
		return NoContent();
	}

	/// <summary>Set All Friend Requests Seen</summary>
	/// <remarks>Sets all friend requests on the authenticated user as seen.</remarks>
	[HttpPut("all-friend-requests-seen")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> SetAllFriendRequestsSeen()
	{
		await _commandDispatcher.DispatchAsync<SetAllFriendRequestsSeen, bool>(new SetAllFriendRequestsSeen
		{
			UserId = CurrentUserId
		});
		return NoContent();
	}

	/// <summary>Set Preferences</summary>
	/// <remarks>Sets the preferences on the authenticated user.</remarks>
	[HttpPut("user-preferences")]
	[InvalidRequest]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> SetPreferences(UserPreferencesResponse request)
	{
		// todo rename SetSettings?
		var command = _mapper.Map<SetPreferences>(request);
		command.UserId = command.UserId;

		await _commandDispatcher.DispatchAsync<SetPreferences, bool>(command);
		return NoContent();
	}

	/// <summary>Set Current Workout</summary>
	/// <remarks>Sets the current workout of the authenticated user to the specified workout, if it exists.</remarks>
	[HttpPut("current-workout")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> SetCurrentWorkout(SetCurrentWorkoutRequest request)
	{
		await _commandDispatcher.DispatchAsync<SetCurrentWorkout, bool>(new SetCurrentWorkout
		{
			UserId = CurrentUserId, WorkoutId = request.WorkoutId
		});
		return NoContent();
	}

	/// <summary>Set All Received Workouts Seen</summary>
	/// <remarks>Sets all received workouts on the authenticated user as seen.</remarks>
	[HttpPut("received-workouts/all-seen")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> SetAllReceivedWorkoutsSeen()
	{
		await _commandDispatcher.DispatchAsync<SetAllReceivedWorkoutsSeen, bool>(new SetAllReceivedWorkoutsSeen
		{
			UserId = CurrentUserId
		});
		return NoContent();
	}

	/// <summary>Set Received Workout Seen</summary>
	/// <remarks>Sets a given received workout on the authenticated user as seen.</remarks>
	/// <param name="sharedWorkoutId">Received workout to set as seen</param>
	[HttpPut("received-workouts/{sharedWorkoutId}/seen")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> SetReceivedWorkoutSeen(string sharedWorkoutId)
	{
		await _commandDispatcher.DispatchAsync<SetReceivedWorkoutSeen, bool>(new SetReceivedWorkoutSeen
		{
			UserId = CurrentUserId, SharedWorkoutId = sharedWorkoutId
		});
		return NoContent();
	}

	/// <summary>Delete Self</summary>
	/// <remarks>
	/// Deletes a user and all data associated with it.<br/><br/>
	/// Data deleted: user, workouts belonging to user, shared workouts sent to user, images belonging to user. The user is also removed as a friend from any other users, and any friend requests they sent are canceled.
	/// </remarks>
	[HttpDelete]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> DeleteSelf()
	{
		await _commandDispatcher.DispatchAsync<DeleteSelf, bool>(new DeleteSelf { UserId = CurrentUserId });
		return NoContent();
	}
}