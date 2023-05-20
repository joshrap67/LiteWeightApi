using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Services.Validation;

public interface IUsersValidator
{
	void ValidSendFriendRequest(User sender, User recipient, string recipientId);
	void ValidShareWorkout(User senderUser, User recipientUser, Workout workoutToSend, string recipientId);
	void ValidAcceptFriendRequest(User acceptedUser, User initiator, string userIdToAccept);
	void ValidRemoveFriend(User userToRemove, string userIdToRemove);
	void ValidCancelFriendRequest(User userToCancel, string userIdToCancel);
	void ValidDeclineFriendRequest(User userToDecline, string userIdToDecline);
}

public class UsersValidator : IUsersValidator
{
	private readonly ICommonValidator _commonValidator;

	public UsersValidator(ICommonValidator commonValidator)
	{
		_commonValidator = commonValidator;
	}

	public void ValidSendFriendRequest(User sender, User recipient, string recipientId)
	{
		_commonValidator.UserExists(recipient, recipientId);
		var senderUserId = sender.Id;

		if (recipient.UserPreferences.PrivateAccount && recipient.Friends.All(x => x.UserId != senderUserId))
		{
			throw new ResourceNotFoundException("User");
		}

		if (sender.Friends.Count >= Globals.MaxNumberFriends)
		{
			throw new MaxLimitException("Max number of friends would be exceeded");
		}

		if (recipient.FriendRequests.Count >= Globals.MaxFriendRequests)
		{
			throw new MaxLimitException($"{recipientId} has too many pending requests");
		}

		if (sender.FriendRequests.Any(x => x.UserId == recipientId))
		{
			throw new MiscErrorException("This user has already sent you a friend request");
		}

		if (senderUserId.Equals(senderUserId, StringComparison.InvariantCultureIgnoreCase))
		{
			throw new MiscErrorException("Cannot send a friend request to yourself");
		}
	}

	public void ValidShareWorkout(User senderUser, User recipientUser, Workout workoutToSend, string recipientId)
	{
		_commonValidator.UserExists(recipientUser, recipientId);
		_commonValidator.ReferencedWorkoutExists(workoutToSend);
		_commonValidator.EnsureWorkoutOwnership(senderUser.Id, workoutToSend);

		var senderId = senderUser.Id;

		if (recipientUser.UserPreferences.PrivateAccount && recipientUser.Friends.All(x => x.UserId != senderId))
		{
			throw new ResourceNotFoundException("User");
		}

		if (recipientUser.ReceivedWorkouts.Count >= Globals.MaxReceivedWorkouts)
		{
			throw new MaxLimitException($"{recipientId} has too many received workouts");
		}

		if (senderUser.WorkoutsSent >= Globals.MaxFreeWorkoutsSent)
		{
			throw new MaxLimitException("You have reached the maximum number of workouts that you can send");
		}

		if (senderId == recipientId)
		{
			throw new MiscErrorException("Cannot send workout to yourself");
		}
	}

	public void ValidAcceptFriendRequest(User acceptedUser, User initiator, string userIdToAccept)
	{
		_commonValidator.UserExists(acceptedUser, userIdToAccept);

		if (initiator.Friends.Count >= Globals.MaxNumberFriends)
		{
			throw new MaxLimitException("Max number of friends reached.");
		}
	}

	public void ValidRemoveFriend(User userToRemove, string userIdToRemove)
	{
		_commonValidator.UserExists(userToRemove, userIdToRemove);
	}

	public void ValidCancelFriendRequest(User userToCancel, string userIdToCancel)
	{
		_commonValidator.UserExists(userToCancel, userIdToCancel);
	}

	public void ValidDeclineFriendRequest(User userToDecline, string userIdToDecline)
	{
		_commonValidator.UserExists(userToDecline, userIdToDecline);
	}
}