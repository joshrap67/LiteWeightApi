using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Services;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Commands.Users.RemoveFriend;

public class RemoveFriendHandler : ICommandHandler<RemoveFriend, bool>
{
	private readonly IRepository _repository;
	private readonly IPushNotificationService _pushNotificationService;

	public RemoveFriendHandler(IRepository repository, IPushNotificationService pushNotificationService)
	{
		_repository = repository;
		_pushNotificationService = pushNotificationService;
	}

	public async Task<bool> HandleAsync(RemoveFriend command)
	{
		var initiator = await _repository.GetUser(command.InitiatorUserId);
		var removedFriend = await _repository.GetUser(command.RemovedUserId);

		ValidationUtils.UserExists(removedFriend);

		var friendToRemove = initiator.Friends.FirstOrDefault(x => x.UserId == command.RemovedUserId);
		var initiatorToRemove = removedFriend.Friends.FirstOrDefault(x => x.UserId == command.InitiatorUserId);
		if (friendToRemove == null) return false;

		initiator.Friends.Remove(friendToRemove);
		removedFriend.Friends.Remove(initiatorToRemove);

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { initiator, removedFriend });

		// send a notification to indicate the user has been removed as a friend
		await _pushNotificationService.SendRemovedAsFriendNotification(removedFriend, initiator);

		return true;
	}
}