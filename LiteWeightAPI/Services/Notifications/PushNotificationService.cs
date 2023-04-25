using System.Text.Json;
using LiteWeightAPI.Domain.Users;

namespace LiteWeightAPI.Services.Notifications;

public interface IPushNotificationService
{
	Task SendWorkoutPushNotification(User recipientUser, SharedWorkoutMeta sharedWorkoutMeta);
}

public class PushNotificationService : IPushNotificationService
{
	private readonly ISnsService _snsService;

	public PushNotificationService(ISnsService snsService)
	{
		_snsService = snsService;
	}

	public async Task SendWorkoutPushNotification(User recipientUser, SharedWorkoutMeta sharedWorkoutMeta)
	{
		await _snsService.SendPushNotification(recipientUser.PushEndpointArn,
			new NotificationData
			{
				Action = SnsService.ReceivedWorkoutAction,
				JsonPayload = JsonSerializer.Serialize(sharedWorkoutMeta)
			});
	}
}