using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Services;
using LiteWeightAPI.Services.Notifications;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Commands.Users.DeclineFriendRequest;

public class DeclineFriendRequestHandler : ICommandHandler<DeclineFriendRequest, bool>
{
	private readonly IRepository _repository;
	private readonly IPushNotificationService _pushNotificationService;

	public DeclineFriendRequestHandler(IRepository repository, IPushNotificationService pushNotificationService)
	{
		_repository = repository;
		_pushNotificationService = pushNotificationService;
	}

	public async Task<bool> HandleAsync(DeclineFriendRequest command)
	{
		var initiator = await _repository.GetUser(command.InitiatorUserId);
		var userToDecline = await _repository.GetUser(command.UserIdToDecline);

		ValidationUtils.UserExists(userToDecline);

		var friendRequest = initiator.FriendRequests.FirstOrDefault(x => x.UserId == command.UserIdToDecline);
		if (friendRequest == null) return false;
		initiator.FriendRequests.Remove(friendRequest);

		var initiatorToRemove = userToDecline.Friends.FirstOrDefault(x => x.UserId == command.InitiatorUserId);
		userToDecline.Friends.Remove(initiatorToRemove);

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { initiator, userToDecline });

		// send a notification to the user who's friend request was declined
		await _pushNotificationService.SendFriendRequestDeclinedNotification(userToDecline, initiator);

		return true;
	}
}