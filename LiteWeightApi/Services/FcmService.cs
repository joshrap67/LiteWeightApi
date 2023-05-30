using FirebaseAdmin.Messaging;
using LiteWeightAPI.Services.Notifications;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Services;

public interface IFcmService
{
	Task SendPushNotification(string targetToken, NotificationData notificationData);
}

public class FcmService : IFcmService
{
	public const string FriendRequestAction = "friendRequest";
	public const string CanceledFriendRequestAction = "canceledFriendRequest";
	public const string AcceptedFriendRequestAction = "acceptedFriendRequest";
	public const string DeclinedFriendRequestAction = "declinedFriendRequest";
	public const string RemovedAsFriendAction = "removedAsFriend";
	public const string ReceivedWorkoutAction = "receivedWorkout";

	public async Task SendPushNotification(string targetToken, NotificationData notificationData)
	{
		if (targetToken == null)
		{
			return;
		}

		var dataJson = JsonUtils.Serialize(notificationData);

		var message = new Message
		{
			Token = targetToken,
			Data = JsonUtils.Deserialize<Dictionary<string, string>>(dataJson)
		};

		await FirebaseMessaging.DefaultInstance.SendAsync(message);
	}
}