using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Services.Validation;

public interface IUsersValidator
{
	void ValidSendFriendRequest(User sender, User recipient, string recipientUsername);
	void ValidShareWorkout(User senderUser, User recipientUser, Workout workoutToSend, string recipientUsername);
	void ValidAcceptFriendRequest(User acceptedUser, User initiator, string usernameToAccept);
	void ValidRemoveFriend(User userToRemove, string usernameToRemove);
	void ValidCancelFriendRequest(User userToCancel, string usernameToCancel);
	void ValidDeclineFriendRequest(User userToDecline, string usernameToDecline);
}

public class UsersValidator : IUsersValidator
{
	private readonly ICommonValidator _commonValidator;

	public UsersValidator(ICommonValidator commonValidator)
	{
		_commonValidator = commonValidator;
	}

	public void ValidSendFriendRequest(User sender, User recipient, string recipientUsername)
	{
		_commonValidator.UserExists(recipient, recipientUsername);
		var senderUsername = sender.Username;
		if (sender.Friends.Count >= Globals.MaxNumberFriends)
		{
			throw new MaxLimitException("Max number of friends would be exceeded");
		}

		if (recipient.FriendRequests.Count >= Globals.MaxFriendRequests)
		{
			throw new MaxLimitException($"{recipientUsername} has too many pending requests");
		}

		if (sender.Friends.Any(x => x.Username == recipientUsername && !x.Confirmed))
		{
			// todo "already sent" error?
			throw new AlreadyExistsException($"A friend request has already been sent to {recipientUsername}");
		}

		if (sender.Friends.Any(x => x.Username == recipientUsername && x.Confirmed))
		{
			throw new MiscErrorException($"You are already friends with {recipientUsername}");
		}

		if (sender.FriendRequests.Any(x => x.Username == recipientUsername))
		{
			throw new MiscErrorException("This user has already sent you a friend request");
		}

		if (senderUsername.Equals(recipientUsername, StringComparison.InvariantCultureIgnoreCase))
		{
			throw new MiscErrorException("Cannot send a friend request to yourself");
		}
	}

	public void ValidShareWorkout(User senderUser, User recipientUser, Workout workoutToSend, string recipientUsername)
	{
		_commonValidator.UserExists(recipientUser, recipientUsername);
		_commonValidator.EnsureWorkoutOwnership(senderUser.Username, workoutToSend);

		var senderUsername = senderUser.Username;

		if (recipientUser.UserPreferences.PrivateAccount &&
		    !recipientUser.Friends.Any(x => x.Username == senderUsername && x.Confirmed))
		{
			throw new UserNotFoundException($"User {recipientUsername} not found");
		}

		if (recipientUser.ReceivedWorkouts.Count >= Globals.MaxReceivedWorkouts)
		{
			throw new MaxLimitException($"{recipientUsername} has too many received workouts");
		}

		if (senderUser.WorkoutsSent >= Globals.MaxFreeWorkoutsSent)
		{
			throw new MaxLimitException("You have reached the maximum number of workouts that you can send");
		}

		if (senderUsername == recipientUsername)
		{
			throw new MiscErrorException("Cannot send workout to yourself");
		}
	}

	public void ValidAcceptFriendRequest(User acceptedUser, User initiator, string usernameToAccept)
	{
		_commonValidator.UserExists(acceptedUser, usernameToAccept);

		if (initiator.Friends.Count >= Globals.MaxNumberFriends)
		{
			throw new MaxLimitException("Max number of friends reached.");
		}
	}

	public void ValidRemoveFriend(User userToRemove, string usernameToRemove)
	{
		_commonValidator.UserExists(userToRemove, usernameToRemove);
	}

	public void ValidCancelFriendRequest(User userToCancel, string usernameToCancel)
	{
		_commonValidator.UserExists(userToCancel, usernameToCancel);
	}

	public void ValidDeclineFriendRequest(User userToDecline, string usernameToDecline)
	{
		_commonValidator.UserExists(userToDecline, usernameToDecline);
	}
}