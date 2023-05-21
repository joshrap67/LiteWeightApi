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
	void ValidAcceptFriendRequest(User acceptedUser, User initiator);
	void ValidRemoveFriend(User userToRemove);
	void ValidCancelFriendRequest(User userToCancel);
	void ValidDeclineFriendRequest(User userToDecline);
	void ValidReportUser(User userToReport);
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
		_commonValidator.UserExists(recipient);
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
		_commonValidator.UserExists(recipientUser);
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

	public void ValidAcceptFriendRequest(User acceptedUser, User initiator)
	{
		_commonValidator.UserExists(acceptedUser);

		if (initiator.Friends.Count >= Globals.MaxNumberFriends)
		{
			throw new MaxLimitException("Max number of friends reached.");
		}
	}

	public void ValidRemoveFriend(User userToRemove)
	{
		_commonValidator.UserExists(userToRemove);
	}

	public void ValidCancelFriendRequest(User userToCancel)
	{
		_commonValidator.UserExists(userToCancel);
	}

	public void ValidDeclineFriendRequest(User userToDecline)
	{
		_commonValidator.UserExists(userToDecline);
	}

	public void ValidReportUser(User userToReport)
	{
		_commonValidator.UserExists(userToReport);
	}
}