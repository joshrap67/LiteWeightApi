using System.ComponentModel.DataAnnotations;
using LiteWeightAPI.Api.Self.Responses;
using LiteWeightAPI.Api.Users.Requests;
using LiteWeightAPI.Api.Users.Responses;
using LiteWeightAPI.Errors.Attributes;
using LiteWeightAPI.Errors.Responses;
using LiteWeightAPI.Imports;
using LiteWeightAPI.Services;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace LiteWeightAPI.Api.Users;

[Route("users")]
[ApiController]
public class UsersController : BaseController
{
	private readonly IUsersService _usersService;

	public UsersController(ILogger logger, IUsersService usersService) : base(logger)
	{
		_usersService = usersService;
	}

	/// <summary>Search by Username</summary>
	/// <remarks>Searches for a user by username. If the user is not found, a 400 response will be returned.</remarks>
	/// <param name="username">Username to search by</param>
	[HttpGet("search")]
	[InvalidRequest, UserNotFound]
	[ProducesResponseType(typeof(SearchUserResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult<SearchUserResponse>> SearchByUsername([FromQuery] [Required] string username)
	{
		var result = await _usersService.SearchByUsername(username, CurrentUserId);
		return result;
	}

	/// <summary>Send Friend Request</summary>
	/// <remarks>Sends a friend request to the specified user, assuming the recipient and current user have not reached maximum allowed friends.</remarks>
	/// <param name="userId">User id of the friend to send the friend request to</param>
	[HttpPut("{userId}/send-friend-request")]
	[MaxLimit, MiscError]
	[PushNotification]
	[ProducesResponseType(typeof(FriendResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<FriendResponse>> SendFriendRequest(string userId)
	{
		var response = await _usersService.SendFriendRequest(userId, CurrentUserId);
		return response;
	}

	/// <summary>Share Workout</summary>
	/// <remarks>Create a shared workout from a specified workout, and send it to a specified user.</remarks>
	/// <param name="request">Request</param>
	/// <param name="userId">User id of the friend to share the workout to</param>
	[HttpPost("{userId}/share-workout")]
	[InvalidRequest, MaxLimit, MiscError, WorkoutNotFound]
	[PushNotification]
	[ProducesResponseType(typeof(ShareWorkoutResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<ShareWorkoutResponse>> ShareWorkout(SendWorkoutRequest request, string userId)
	{
		var createdWorkoutId = await _usersService.ShareWorkout(request, userId, CurrentUserId);
		return new ShareWorkoutResponse { SharedWorkoutId = createdWorkoutId };
	}

	/// <summary>Accept Friend Request</summary>
	/// <remarks>Accepts a friend request from the specified user.</remarks>
	/// <param name="userId">User id of the friend to accept as a friend</param>
	[HttpPut("{userId}/accept-friend-request")]
	[MaxLimit]
	[PushNotification]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> AcceptFriendRequest(string userId)
	{
		await _usersService.AcceptFriendRequest(userId, CurrentUserId);
		return Ok();
	}

	/// <summary>Remove Friend</summary>
	/// <remarks>Removes the specified user from the current user's friend's list. This action also removes the current user from the specified user's friend's list.</remarks>
	/// <param name="userId">User id of the user to remove as a friend</param>
	[HttpDelete("{userId}/friend")]
	[PushNotification]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> RemoveFriend(string userId)
	{
		await _usersService.RemoveFriend(userId, CurrentUserId);
		return Ok();
	}

	/// <summary>Cancel Friend Request</summary>
	/// <remarks>Cancels a friend request sent to the specified user.</remarks>
	/// <param name="userId">User id of the user to cancel the friend request of</param>
	[HttpPut("{userId}/cancel-friend-request")]
	[PushNotification]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> CancelFriendRequest(string userId)
	{
		await _usersService.CancelFriendRequest(userId, CurrentUserId);
		return Ok();
	}

	/// <summary>Decline Friend Request</summary>
	/// <remarks>Declines a friend request that was sent from the specified user.</remarks>
	/// <param name="userId">User id of the user to cancel the pending friend request</param>
	[HttpPut("{userId}/decline-friend-request")]
	[PushNotification]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> DeclineFriendRequest(string userId)
	{
		await _usersService.DeclineFriendRequest(userId, CurrentUserId);
		return Ok();
	}

	/// <summary>Report User</summary>
	/// <remarks>Reports a user for the developers to review.</remarks>
	/// <param name="userId">User id of the user to report</param>
	[HttpPost("{userId}/report")]
	[ProducesResponseType(typeof(ComplaintResponse), 200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<ComplaintResponse>> Report(string userId)
	{
		var response = await _usersService.ReportUser(userId, CurrentUserId);
		return response;
	}
}