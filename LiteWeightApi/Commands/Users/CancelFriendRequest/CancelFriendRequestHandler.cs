using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Services;
using LiteWeightAPI.Services.Notifications;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Commands.Users.CancelFriendRequest;

public class CancelFriendRequestHandler : ICommandHandler<CancelFriendRequest, bool>
{
	private readonly IRepository _repository;
	private readonly IPushNotificationService _pushNotificationService;

	public CancelFriendRequestHandler(IRepository repository, IPushNotificationService pushNotificationService)
	{
		_repository = repository;
		_pushNotificationService = pushNotificationService;
	}

	public async Task<bool> HandleAsync(CancelFriendRequest command)
	{
		var initiator = await _repository.GetUser(command.InitiatorUserId);
		var userToCancel = await _repository.GetUser(command.UserIdToCancel);

		CommonValidator.UserExists(userToCancel);

		var pendingFriend = initiator.Friends.FirstOrDefault(x => x.UserId == command.UserIdToCancel);
		if (pendingFriend == null)
		{
			return false;
		}

		initiator.Friends.Remove(pendingFriend);
		userToCancel.FriendRequests.RemoveAll(x => x.UserId == command.InitiatorUserId);

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { initiator, userToCancel });

		// send a notification to the canceled user
		await _pushNotificationService.SendFriendRequestCanceledNotification(userToCancel, initiator);

		return true;
	}
}