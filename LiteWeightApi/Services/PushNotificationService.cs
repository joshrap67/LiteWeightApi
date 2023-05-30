using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Services.Notifications;
using LiteWeightAPI.Services.Notifications.Responses;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Services;

public interface IPushNotificationService
{
	Task SendReceivedWorkoutPushNotification(User recipientUser, SharedWorkoutInfo sharedWorkoutInfo);
	Task SendNewFriendRequestNotification(User recipientUser, FriendRequest friendRequest);
	Task SendFriendRequestAcceptedNotification(User acceptedUser, User initiator);
	Task SendFriendRequestCanceledNotification(User canceledUser, User initiator);
	Task SendFriendRequestDeclinedNotification(User declinedUser, User initiator);
	Task SendRemovedAsFriendNotification(User removedFriend, User initiator);
}

public class PushNotificationService : IPushNotificationService
{
	private readonly IFcmService _fcmService;

	public PushNotificationService(IFcmService fcmService)
	{
		_fcmService = fcmService;
	}

	public async Task SendReceivedWorkoutPushNotification(User recipientUser, SharedWorkoutInfo sharedWorkoutInfo)
	{
		await _fcmService.SendPushNotification(recipientUser.FirebaseMessagingToken,
			new NotificationData
			{
				Action = FcmService.ReceivedWorkoutAction,
				JsonPayload = JsonUtils.Serialize(sharedWorkoutInfo)
			});
	}

	public async Task SendNewFriendRequestNotification(User recipientUser, FriendRequest friendRequest)
	{
		await _fcmService.SendPushNotification(recipientUser.FirebaseMessagingToken,
			new NotificationData
			{
				Action = FcmService.FriendRequestAction,
				JsonPayload = JsonUtils.Serialize(friendRequest)
			});
	}

	public async Task SendFriendRequestAcceptedNotification(User acceptedUser, User initiator)
	{
		await _fcmService.SendPushNotification(acceptedUser.FirebaseMessagingToken,
			new NotificationData
			{
				Action = FcmService.AcceptedFriendRequestAction,
				JsonPayload = JsonUtils.Serialize(new AcceptedFriendRequestResponse
					{ UserId = initiator.Id, Username = initiator.Username })
			});
	}

	public async Task SendFriendRequestCanceledNotification(User canceledUser, User initiator)
	{
		await _fcmService.SendPushNotification(canceledUser.FirebaseMessagingToken,
			new NotificationData
			{
				Action = FcmService.CanceledFriendRequestAction,
				JsonPayload = JsonUtils.Serialize(new CanceledFriendRequestResponse
					{ UserId = initiator.Id, Username = initiator.Username })
			});
	}

	public async Task SendFriendRequestDeclinedNotification(User declinedUser, User initiator)
	{
		await _fcmService.SendPushNotification(declinedUser.FirebaseMessagingToken,
			new NotificationData
			{
				Action = FcmService.DeclinedFriendRequestAction,
				JsonPayload = JsonUtils.Serialize(new DeclinedFriendRequestResponse
					{ UserId = initiator.Id, Username = initiator.Username })
			});
	}

	public async Task SendRemovedAsFriendNotification(User removedFriend, User initiator)
	{
		await _fcmService.SendPushNotification(removedFriend.FirebaseMessagingToken,
			new NotificationData
			{
				Action = FcmService.RemovedAsFriendAction,
				JsonPayload = JsonUtils.Serialize(new RemovedAsFriendResponse
					{ UserId = initiator.Id, Username = initiator.Username })
			});
	}
}