using LiteWeightAPI.Domain;
using LiteWeightAPI.Errors.Exceptions;

namespace LiteWeightAPI.Commands.Self.SetUsername;

public class SetUsernameHandler : ICommandHandler<SetUsername, bool>
{
	private readonly IRepository _repository;

	public SetUsernameHandler(IRepository repository)
	{
		_repository = repository;
	}

	public async Task<bool> HandleAsync(SetUsername command)
	{
		var user = await _repository.GetUser(command.UserId);
		if (command.NewUsername == user.Username)
		{
			return false;
		}

		var searchByUsername = await _repository.GetUserByUsername(command.NewUsername);
		if (searchByUsername != null)
		{
			throw new AlreadyExistsException("User already exists with this username");
		}

		user.Username = command.NewUsername;

		// due to potential limits on transactions, this is not atomic :(
		// update any friend requests the user sent to have this new username
		var usersWhoReceivedFriendRequests = user.Friends.Where(x => !x.Confirmed).Select(x => x.UserId);
		foreach (var userId in usersWhoReceivedFriendRequests)
		{
			var recipient = await _repository.GetUser(userId);
			var friendRequest = recipient.FriendRequests.FirstOrDefault(x => x.UserId == command.UserId);
			if (friendRequest == null) continue;

			friendRequest.Username = command.NewUsername;
			await _repository.PutUser(recipient);
		}
		
		
		var usersWhoSentFriendRequests = user.FriendRequests.Select(x => x.UserId);
		foreach (var userId in usersWhoSentFriendRequests)
		{
			var sender = await _repository.GetUser(userId);
			var friend = sender.Friends.FirstOrDefault(x => x.UserId == command.UserId);
			if (friend == null) continue;

			friend.Username = command.NewUsername;
			await _repository.PutUser(sender);
		}

		// for all users that are friends with this user, update with the new username
		var usersWhoAreFriends = user.Friends.Where(x=>x.Confirmed).Select(x => x.UserId);
		foreach (var userId in usersWhoAreFriends)
		{
			var sender = await _repository.GetUser(userId);
			var friend = sender.Friends.FirstOrDefault(x => x.UserId == command.UserId);
			if (friend == null) continue;

			friend.Username = command.NewUsername;
			await _repository.PutUser(sender);
		}
		
		await _repository.PutUser(user);

		return true;
	}
}