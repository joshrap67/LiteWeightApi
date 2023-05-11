﻿using System.Text.Json;
using FirebaseAdmin.Messaging;

namespace LiteWeightAPI.Services.Notifications;

public interface IFcmService
{
	Task SendFeedbackEmail(string arn, string subject, string body);
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

	public FcmService()
	{
	}

	public async Task SendFeedbackEmail(string arn, string subject, string body)
	{
	}

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