using AutoMapper;
using LiteWeightAPI.Api.CurrentUser.Responses;
using LiteWeightAPI.Api.Users.Requests;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Services.Helpers;
using LiteWeightAPI.Services.Notifications;
using LiteWeightAPI.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace LiteWeightAPI.Services;

public interface IUsersService
{
	Task<FriendResponse> SendFriendRequest(SendFriendRequestRequest request, string senderUserId);
	Task<string> ShareWorkout(SendWorkoutRequest request, string senderUserId);
	Task AcceptFriendRequest(string acceptedUserId, string initiatorUserId);
	Task RemoveFriend(string userIdToRemove, string initiatorUserId);
	Task CancelFriendRequest(string userIdToCancel, string initiatorUserId);
	Task DeclineFriendRequest(string userIdToDecline, string initiatorUserId);
}

public class UsersService : IUsersService
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;
	private readonly IClock _clock;
	private readonly IUsersValidator _usersValidator;
	private readonly IPushNotificationService _pushNotificationService;

	public UsersService(IRepository repository, IMapper mapper, IClock clock, IUsersValidator usersValidator,
		IPushNotificationService pushNotificationService)
	{
		_repository = repository;
		_mapper = mapper;
		_clock = clock;
		_usersValidator = usersValidator;
		_pushNotificationService = pushNotificationService;
	}

	public async Task<FriendResponse> SendFriendRequest(SendFriendRequestRequest request, string senderUserId)
	{
		var senderUser = await _repository.GetUser(senderUserId);
		var recipientUsername = request.RecipientUsername;
		var recipientUser = await _repository.GetUserByUsername(recipientUsername);

		_usersValidator.ValidSendFriendRequest(senderUser, recipientUser, recipientUsername);

		var friendToAdd = new Friend { Username = recipientUsername, UserId = recipientUser.Id, UserIcon = recipientUser.Icon };
		var now = _clock.GetCurrentInstant();
		var friendRequest = new FriendRequest
		{
			UserId = senderUserId,
			Username = senderUser.Username,
			UserIcon = senderUser.Icon,
			SentUtc = now
		};
		senderUser.Friends.Add(friendToAdd);
		recipientUser.FriendRequests.Add(friendRequest);

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { senderUser, recipientUser });

		// send a notification to the user that received the friend request
		await _pushNotificationService.SendNewFriendRequestNotification(recipientUser, friendRequest);

		return _mapper.Map<FriendResponse>(friendToAdd);
	}

	public async Task<string> ShareWorkout(SendWorkoutRequest request, string senderUserId)
	{
		var senderUser = await _repository.GetUser(senderUserId);
		var recipientUsername = request.Username;
		var recipientUser = await _repository.GetUserByUsername(recipientUsername);
		var workoutToSend = await _repository.GetWorkout(request.WorkoutId);

		_usersValidator.ValidShareWorkout(senderUser, recipientUser, workoutToSend, recipientUsername);

		var sharedWorkoutId = Guid.NewGuid().ToString();
		var sharedWorkoutInfo = new SharedWorkoutInfo
		{
			SharedWorkoutId = sharedWorkoutId,
			SenderId = senderUserId,
			SenderUsername = senderUser.Username,
			SenderIcon = senderUser.Icon,
			WorkoutName = workoutToSend.Name,
			SharedUtc = _clock.GetCurrentInstant(),
			TotalDays = workoutToSend.Routine.TotalNumberOfDays,
			MostFrequentFocus = WorkoutHelper.FindMostFrequentFocus(senderUser, workoutToSend.Routine)
		};
		recipientUser.ReceivedWorkouts.Add(sharedWorkoutInfo);
		senderUser.WorkoutsSent++;

		var sharedWorkout = new SharedWorkout(workoutToSend, recipientUsername, sharedWorkoutId, senderUser);

		await _repository.ExecuteBatchWrite(
			usersToPut: new List<User> { senderUser, recipientUser },
			sharedWorkoutsToPut: new List<SharedWorkout> { sharedWorkout }
		);

		await _pushNotificationService.SendWorkoutPushNotification(recipientUser, sharedWorkoutInfo);

		return sharedWorkoutId;
	}

	public async Task AcceptFriendRequest(string acceptedUserId, string initiatorUserId)
	{
		var initiator = await _repository.GetUser(initiatorUserId); // todo confusing name?
		var acceptedUser = await _repository.GetUser(acceptedUserId);

		_usersValidator.ValidAcceptFriendRequest(acceptedUser, initiator, initiatorUserId);

		var friendRequest = initiator.FriendRequests.FirstOrDefault(x => x.UserId == acceptedUserId);
		if (friendRequest == null)
		{
			return;
		}

		// remove request from user who initiated, and add the new friend
		var newFriend = new Friend
		{
			UserId = acceptedUser.Id,
			Username = acceptedUser.Username,
			UserIcon = acceptedUser.Icon,
			Confirmed = true
		};
		initiator.FriendRequests.Remove(friendRequest);
		initiator.Friends.Add(newFriend);
		// update friend to accepted for the user who sent the request
		acceptedUser.Friends.First(x => x.UserId == acceptedUserId).Confirmed = true;

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { initiator, acceptedUser });

		// send a notification to the user who was accepted as a friend
		await _pushNotificationService.SendFriendRequestAcceptedNotification(acceptedUser, initiator);
	}

	public async Task RemoveFriend(string userIdToRemove, string initiatorUserId)
	{
		var initiator = await _repository.GetUser(initiatorUserId);
		var removedFriend = await _repository.GetUser(userIdToRemove);

		_usersValidator.ValidRemoveFriend(removedFriend, userIdToRemove);

		var friendToRemove = initiator.Friends.FirstOrDefault(x => x.UserId == userIdToRemove);
		var initiatorToRemove = removedFriend.Friends.FirstOrDefault(x => x.UserId == initiatorUserId);
		if (friendToRemove == null)
		{
			return;
		}

		initiator.Friends.Remove(friendToRemove);
		removedFriend.Friends.Remove(initiatorToRemove);

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { initiator, removedFriend });

		// send a notification to indicate the user has been removed as a friend
		await _pushNotificationService.SendRemovedAsFriendNotification(removedFriend, initiator);
	}

	public async Task CancelFriendRequest(string userIdToCancel, string initiatorUserId)
	{
		var initiator = await _repository.GetUser(initiatorUserId);
		var userToCancel = await _repository.GetUser(userIdToCancel);

		_usersValidator.ValidCancelFriendRequest(userToCancel, userIdToCancel);

		var pendingFriend = initiator.Friends.FirstOrDefault(x => x.UserId == userIdToCancel);
		if (pendingFriend == null)
		{
			return;
		}

		initiator.Friends.Remove(pendingFriend);
		userToCancel.FriendRequests.RemoveAll(x => x.UserId == initiatorUserId);

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { initiator, userToCancel });

		// send a notification to the canceled user
		await _pushNotificationService.SendFriendRequestCanceledNotification(userToCancel, initiator);
	}

	public async Task DeclineFriendRequest(string userIdToDecline, string initiatorUserId)
	{
		var initiator = await _repository.GetUser(initiatorUserId);
		var userToDecline = await _repository.GetUser(userIdToDecline);

		_usersValidator.ValidDeclineFriendRequest(userToDecline, userIdToDecline);

		var friendRequest = initiator.FriendRequests.FirstOrDefault(x => x.UserId == userIdToDecline);
		if (friendRequest == null)
		{
			return;
		}

		initiator.FriendRequests.Remove(friendRequest);
		userToDecline.Friends.RemoveAll(x => x.UserId == initiatorUserId);

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { initiator, userToDecline });

		// send a notification to the user who's friend request was declined
		await _pushNotificationService.SendFriendRequestDeclinedNotification(userToDecline, initiator);
	}
}