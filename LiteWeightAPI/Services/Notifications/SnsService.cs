using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace LiteWeightAPI.Services.Notifications;

public interface ISnsService
{
	Task<string> RegisterTokenForPushPlatform(string gcmToken, string currentArn, string username);
	Task SendFeedbackEmail(string arn, string subject, string body);
	Task DeletePushEndpoint(string currentArn);
	Task SendPushNotification(string targetArn, NotificationData notificationData);
}

public class SnsService : ISnsService
{
	private readonly AmazonSimpleNotificationServiceClient _client;

	private const string PushSnsPlatformArn = "arn:aws:sns:us-east-1:438338746171:app/GCM/LiteWeight";
	private const string PushEmailPlatformArn = "arn:aws:sns:us-east-1:438338746171:LiteWeightDevEmail";

	public const string FriendRequestAction = "friendRequest";
	public const string CanceledFriendRequestAction = "canceledFriendRequest";
	public const string AcceptedFriendRequestAction = "acceptedFriendRequest";
	public const string DeclinedFriendRequestAction = "declinedFriendRequest";
	public const string RemovedAsFriendAction = "removedAsFriend";
	public const string ReceivedWorkoutAction = "receivedWorkout";

	public SnsService()
	{
		_client = new AmazonSimpleNotificationServiceClient(region: Amazon.RegionEndpoint.USEast1); // todo env var
	}

	public async Task<string> RegisterTokenForPushPlatform(string gcmToken, string currentArn, string username)
	{
		var request = new CreatePlatformEndpointRequest
		{
			PlatformApplicationArn = PushSnsPlatformArn,
			Token = gcmToken,
			CustomUserData = username
		};
		var response = await _client.CreatePlatformEndpointAsync(request);
		var endpointArn = response.EndpointArn;
		if (endpointArn != currentArn)
		{
			// user's GCM token has changed so delete the old stale push platform endpoint
			await DeletePushEndpoint(currentArn);
		}

		return endpointArn;
	}

	public async Task DeletePushEndpoint(string currentArn)
	{
		await _client.DeleteEndpointAsync(new DeleteEndpointRequest { EndpointArn = currentArn });
	}

	public async Task SendFeedbackEmail(string arn, string subject, string body)
	{
		var publishRequest = new PublishRequest(arn, body, subject);
		await _client.PublishAsync(publishRequest);
	}

	public async Task SendPushNotification(string targetArn, NotificationData notificationData)
	{
		if (targetArn == null)
		{
			return;
		}

		var message = new NotificationMessage
		{
			InnerMsg = new NotificationMessage.InnerMessage
			{
				Metadata = JsonSerializer.Serialize(notificationData)
			}
		};

		var publishRequest = new PublishRequest
		{
			TargetArn = targetArn,
			MessageStructure = "json",
			Message = JsonSerializer.Serialize(message)
		};

		await _client.PublishAsync(publishRequest);
	}
}