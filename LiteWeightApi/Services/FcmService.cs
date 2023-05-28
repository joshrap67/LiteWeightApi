using System.Text.Json;
using FirebaseAdmin.Messaging;
using LiteWeightAPI.Services.Notifications;

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

		var data = new NotificationMessage
		{
			InnerMsg = new NotificationMessage.InnerMessage
			{
				Metadata = JsonSerializer.Serialize(notificationData)
			}
		};

		var dataJson = JsonSerializer.Serialize(data);

		var message = new Message
		{
			Token = targetToken,
			Data = JsonSerializer.Deserialize<Dictionary<string, string>>(dataJson)
		};

		await FirebaseMessaging.DefaultInstance.SendAsync(message);
	}
}