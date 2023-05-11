using System.Text.Json;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Services.Notifications.Responses;

namespace LiteWeightAPI.Services.Notifications;

public interface IPushNotificationService
{
	Task SendWorkoutPushNotification(User recipientUser, SharedWorkoutInfo sharedWorkoutInfo);
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

	public async Task SendWorkoutPushNotification(User recipientUser, SharedWorkoutInfo sharedWorkoutInfo)
	{
		await _fcmService.SendPushNotification(recipientUser.PushEndpointArn,
			new NotificationData
			{
				Action = FcmService.ReceivedWorkoutAction,
				JsonPayload = JsonSerializer.Serialize(sharedWorkoutInfo)
			});
	}

	public async Task SendNewFriendRequestNotification(User recipientUser, FriendRequest friendRequest)
	{
		await _fcmService.SendPushNotification(recipientUser.PushEndpointArn,
			new NotificationData
			{
				Action = FcmService.FriendRequestAction,
				JsonPayload = JsonSerializer.Serialize(friendRequest)
			});
	}

	public async Task SendFriendRequestAcceptedNotification(User acceptedUser, User initiator)
	{
		await _fcmService.SendPushNotification(acceptedUser.PushEndpointArn,
			new NotificationData
			{
				Action = FcmService.AcceptedFriendRequestAction,
				JsonPayload = JsonSerializer.Serialize(new AcceptedFriendRequestResponse
					{ UserId = initiator.Id, Username = initiator.Username })
			});
	}

	public async Task SendFriendRequestCanceledNotification(User canceledUser, User initiator)
	{
		await _fcmService.SendPushNotification(canceledUser.PushEndpointArn,
			new NotificationData
			{
				Action = FcmService.CanceledFriendRequestAction,
				JsonPayload = JsonSerializer.Serialize(new CanceledFriendRequestResponse
					{ UserId = initiator.Id, Username = initiator.Username })
			});
	}

	public async Task SendFriendRequestDeclinedNotification(User declinedUser, User initiator)
	{
		await _fcmService.SendPushNotification(declinedUser.PushEndpointArn,
			new NotificationData
			{
				Action = FcmService.DeclinedFriendRequestAction,
				JsonPayload = JsonSerializer.Serialize(new DeclinedFriendRequestResponse
					{ UserId = initiator.Id, Username = initiator.Username })
			});
	}

	public async Task SendRemovedAsFriendNotification(User removedFriend, User initiator)
	{
		await _fcmService.SendPushNotification(removedFriend.PushEndpointArn,
			new NotificationData
			{
				Action = FcmService.RemovedAsFriendAction,
				JsonPayload = JsonSerializer.Serialize(new RemovedAsFriendResponse
					{ UserId = initiator.Id, Username = initiator.Username })
			});
	}
}