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
using LiteWeightAPI.Commands.Self.SetFirebaseMessagingToken;
using LiteWeightAPI.Commands.Self.SetReceivedWorkoutSeen;
using LiteWeightAPI.Commands.Self.SetSettings;
using LiteWeightAPI.Commands.Self.UpdateProfilePicture;
using LiteWeightAPI.Errors.Attributes;
using LiteWeightAPI.Imports;
using Microsoft.AspNetCore.Mvc;

namespace LiteWeightAPI.Api.Self;

[Route("self")]
[ApiController]
public class SelfController : BaseController
{
	private readonly ICommandDispatcher _dispatcher;
	private readonly IMapper _mapper;

	public SelfController(ICommandDispatcher dispatcher, IMapper mapper, Serilog.ILogger logger) : base(logger)
	{
		_dispatcher = dispatcher;
		_mapper = mapper;
	}

	/// <summary>Get Self</summary>
	/// <remarks>Returns the user that is currently authenticated.</remarks>
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<UserResponse>> GetSelf()
	{
		var user = await _dispatcher.DispatchAsync<GetSelf, UserResponse>(new GetSelf
		{
			UserId = CurrentUserId
		});
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
	public async Task<ActionResult<UserResponse>> CreateSelf(CreateSelfRequest request)
	{
		var command = _mapper.Map<CreateSelf>(request);
		command.UserEmail = CurrentUserEmail;
		command.UserId = CurrentUserId;

		var user = await _dispatcher.DispatchAsync<CreateSelf, UserResponse>(command);
		return new CreatedResult(new Uri("/self", UriKind.Relative), user);
	}

	/// <summary>Update Profile Picture</summary>
	/// <remarks>Updates the user's profile picture. Note that it simply replaces the old picture using the same image url.</remarks>
	[HttpPut("profile-picture")]
	[InvalidRequest]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> UpdateProfilePicture(UpdateProfilePictureRequest request)
	{
		await _dispatcher.DispatchAsync<UpdateProfilePicture, bool>(new UpdateProfilePicture
		{
			UserId = CurrentUserId, ImageData = request.ProfilePictureData
		});
		return NoContent();
	}

	/// <summary>Set Firebase Messaging Token</summary>
	/// <remarks>Sets the firebase messaging token to the authenticated user. This enables the authenticated user's ability to receive push notifications, or removes it if the token is null.</remarks>
	[HttpPut("set-firebase-messaging-token")]
	[InvalidRequest]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> SetFirebaseMessagingToken(SetFirebaseMessagingTokenRequest request)
	{
		await _dispatcher.DispatchAsync<SetFirebaseMessagingToken, bool>(new SetFirebaseMessagingToken
		{
			UserId = CurrentUserId, Token = request.FirebaseMessagingToken
		});
		return NoContent();
	}

	/// <summary>Set All Friend Requests Seen</summary>
	/// <remarks>Sets all friend requests on the authenticated user as seen.</remarks>
	[HttpPut("all-friend-requests-seen")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> SetAllFriendRequestsSeen()
	{
		await _dispatcher.DispatchAsync<SetAllFriendRequestsSeen, bool>(new SetAllFriendRequestsSeen
		{
			UserId = CurrentUserId
		});
		return NoContent();
	}

	/// <summary>Set Settings</summary>
	/// <remarks>Sets the settings on the authenticated user.</remarks>
	[HttpPut("settings")]
	[InvalidRequest]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> SetSettings(UserSettingsResponse request)
	{
		var command = _mapper.Map<SetSettings>(request);
		command.UserId = CurrentUserId;

		await _dispatcher.DispatchAsync<SetSettings, bool>(command);
		return NoContent();
	}

	/// <summary>Set Current Workout</summary>
	/// <remarks>Sets the current workout of the authenticated user to the specified workout, if it exists.</remarks>
	[HttpPut("current-workout")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> SetCurrentWorkout(SetCurrentWorkoutRequest request)
	{
		await _dispatcher.DispatchAsync<SetCurrentWorkout, bool>(new SetCurrentWorkout
		{
			UserId = CurrentUserId, CurrentWorkoutId = request.CurrentWorkoutId
		});
		return NoContent();
	}

	/// <summary>Set All Received Workouts Seen</summary>
	/// <remarks>Sets all received workouts on the authenticated user as seen.</remarks>
	[HttpPut("received-workouts/all-seen")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<ActionResult> SetAllReceivedWorkoutsSeen()
	{
		await _dispatcher.DispatchAsync<SetAllReceivedWorkoutsSeen, bool>(new SetAllReceivedWorkoutsSeen
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
		await _dispatcher.DispatchAsync<SetReceivedWorkoutSeen, bool>(new SetReceivedWorkoutSeen
		{
			UserId = CurrentUserId, SharedWorkoutId = sharedWorkoutId
		});
		return NoContent();
	}

	/// <summary>Delete Self</summary>
	/// <remarks>
	/// Deletes a user and all data associated with it.<br/><br/>
	/// Data deleted: user, workouts belonging to user, images belonging to user. The user is also removed as a friend from any other users, and any friend requests they sent are canceled.
	/// </remarks>
	[HttpDelete]
	[PushNotification]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> DeleteSelf()
	{
		await _dispatcher.DispatchAsync<DeleteSelf, bool>(new DeleteSelf { UserId = CurrentUserId });
		return NoContent();
	}
}