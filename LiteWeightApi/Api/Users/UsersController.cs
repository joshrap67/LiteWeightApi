using System.ComponentModel.DataAnnotations;
using LiteWeightAPI.Api.Self.Responses;
using LiteWeightAPI.Api.Users.Requests;
using LiteWeightAPI.Api.Users.Responses;
using LiteWeightAPI.Commands;
using LiteWeightAPI.Commands.Users.AcceptFriendRequest;
using LiteWeightAPI.Commands.Users.CancelFriendRequest;
using LiteWeightAPI.Commands.Users.DeclineFriendRequest;
using LiteWeightAPI.Commands.Users.RemoveFriend;
using LiteWeightAPI.Commands.Users.ReportUser;
using LiteWeightAPI.Commands.Users.SearchByUsername;
using LiteWeightAPI.Commands.Users.SendFriendRequest;
using LiteWeightAPI.Commands.Users.ShareWorkout;
using LiteWeightAPI.Errors.Attributes;
using LiteWeightAPI.Errors.Attributes.Setup;
using LiteWeightAPI.Errors.Responses;
using LiteWeightAPI.Imports;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace LiteWeightAPI.Api.Users;

[Route("users")]
[ApiController]
public class UsersController : BaseController
{
	private readonly ICommandDispatcher _dispatcher;

	public UsersController(ILogger logger, ICommandDispatcher dispatcher) : base(logger)
	{
		_dispatcher = dispatcher;
	}

	/// <summary>Search by Username</summary>
	/// <remarks>Searches for a user by username. If the user is not found, a 400 response will be returned.</remarks>
	/// <param name="username">Username to search by</param>
	[HttpGet("search")]
	[InvalidRequest, UserNotFound]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<SearchUserResponse>> SearchByUsername([FromQuery] [Required] string username)
	{
		var result = await _dispatcher.DispatchAsync<SearchByUsername, SearchUserResponse>(new SearchByUsername
		{
			Username = username,
			InitiatorId = CurrentUserId
		});

		// breaking exception pattern since this is not a "real" error that should be logged
		if (result == null)
		{
			return BadRequest(new BadRequestResponse
			{
				Message = "User not found",
				ErrorType = ErrorTypes.UserNotFound
			});
		}

		return result;
	}

	/// <summary>Send Friend Request</summary>
	/// <remarks>Sends a friend request to the specified user, assuming the recipient and authenticated user have not reached the maximum allowed friends.</remarks>
	/// <param name="userId">User id of the user to send the friend request to</param>
	[HttpPut("{userId}/send-friend-request")]
	[MaxLimit, MiscError]
	[PushNotification]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<FriendResponse>> SendFriendRequest(string userId)
	{
		var response = await _dispatcher.DispatchAsync<SendFriendRequest, FriendResponse>(new SendFriendRequest
		{
			SenderId = CurrentUserId,
			RecipientId = userId
		});
		return response;
	}

	/// <summary>Share Workout</summary>
	/// <remarks>Create a shared workout from a specified workout, and send it to the specified user.</remarks>
	/// <param name="request">Request</param>
	/// <param name="userId">User id of the user to share the workout to</param>
	[HttpPost("{userId}/share-workout")]
	[InvalidRequest, MaxLimit, MiscError, WorkoutNotFound]
	[PushNotification]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ShareWorkoutResponse>> ShareWorkout(ShareWorkoutRequest request, string userId)
	{
		var createdWorkoutId = await _dispatcher.DispatchAsync<ShareWorkout, string>(new ShareWorkout
		{
			WorkoutId = request.WorkoutId,
			RecipientUserId = userId,
			SenderUserId = CurrentUserId
		});
		return new ObjectResult(new ShareWorkoutResponse { SharedWorkoutId = createdWorkoutId })
			{ StatusCode = StatusCodes.Status201Created };
	}

	/// <summary>Accept Friend Request</summary>
	/// <remarks>Accepts a friend request from the specified user, assuming authenticated user does not have maximum amount of friends allowed.</remarks>
	/// <param name="userId">User id of the user to accept as a friend</param>
	[HttpPut("{userId}/accept-friend-request")]
	[MaxLimit]
	[PushNotification]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> AcceptFriendRequest(string userId)
	{
		await _dispatcher.DispatchAsync<AcceptFriendRequest, bool>(new AcceptFriendRequest
		{
			InitiatorUserId = CurrentUserId,
			AcceptedUserId = userId
		});
		return NoContent();
	}

	/// <summary>Remove Friend</summary>
	/// <remarks>Removes the specified user from the authenticated user's friend's list. This action also removes the authenticated user from the specified user's friend's list.</remarks>
	/// <param name="userId">User id of the user to remove as a friend</param>
	[HttpDelete("{userId}/friend")]
	[PushNotification]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> RemoveFriend(string userId)
	{
		await _dispatcher.DispatchAsync<RemoveFriend, bool>(new RemoveFriend
		{
			InitiatorUserId = CurrentUserId,
			RemovedUserId = userId
		});
		return NoContent();
	}

	/// <summary>Cancel Friend Request</summary>
	/// <remarks>Cancels a friend request sent to the specified user.</remarks>
	/// <param name="userId">User id of the user to cancel the friend request of</param>
	[HttpPut("{userId}/cancel-friend-request")]
	[PushNotification]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> CancelFriendRequest(string userId)
	{
		await _dispatcher.DispatchAsync<CancelFriendRequest, bool>(new CancelFriendRequest
		{
			InitiatorUserId = CurrentUserId,
			UserIdToCancel = userId
		});
		return NoContent();
	}

	/// <summary>Decline Friend Request</summary>
	/// <remarks>Declines a friend request that was sent from the specified user. This action removes the authenticated user from the specified user's friend's list.</remarks>
	/// <param name="userId">User id of the user to cancel the pending friend request</param>
	[HttpPut("{userId}/decline-friend-request")]
	[PushNotification]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> DeclineFriendRequest(string userId)
	{
		await _dispatcher.DispatchAsync<DeclineFriendRequest, bool>(new DeclineFriendRequest
		{
			InitiatorUserId = CurrentUserId,
			UserIdToDecline = userId
		});
		return NoContent();
	}

	/// <summary>Report User</summary>
	/// <remarks>Creates a complaint against the specified user for the developers to review.</remarks>
	/// <param name="userId">User id of the user to report</param>
	/// <param name="request">Request</param>
	[HttpPost("{userId}/report")]
	[InvalidRequest]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ComplaintResponse>> Report(string userId, ReportUserRequest request)
	{
		var response = await _dispatcher.DispatchAsync<ReportUser, ComplaintResponse>(new ReportUser
		{
			InitiatorUserId = CurrentUserId,
			ReportedUserId = userId,
			Description = request.Description
		});
		return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
	}
}