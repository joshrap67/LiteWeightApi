using LiteWeightAPI.Api.Users.Requests;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Imports;

// ReSharper disable SimplifyLinqExpressionUseAll

namespace LiteWeightAPI.Services.Validation;

public interface IUserValidator
{
	Task ValidCreateUser(string username);
	void ValidAcceptFriendRequest(User sender, User recipient);
	void ValidCancelFriendRequest(User user, User userToCancel);
	void ValidDeclineFriendRequest(User user, User userToDecline);
	void ValidRemoveFriend(User user, User userToRemove);
	void ValidBlockFriend(User user, User userToBlock);
	void ValidSendFriendRequest(User sender, User recipient);
	void ValidUpdateOwnedExercise(UpdateExerciseRequest request, User activeUser);
	void ValidNewExercise(CreateExerciseRequest request, User user);
	void ValidRestartStatistics(User user, string workoutId);
}

public class UserValidator : IUserValidator
{
	private readonly IRepository _repository;

	public UserValidator(IRepository repository)
	{
		_repository = repository;
	}

	public async Task ValidCreateUser(string username)
	{
		var user = await _repository.GetUser(username);
		if (user != null)
		{
			throw new Exception("User already exists");
		}
	}

	public void ValidAcceptFriendRequest(User sender, User recipient)
	{
		var senderUsername = sender.Username;
		var recipientUsername = recipient.Username;
		if (!recipient.FriendRequests.Any(x => x.Username == senderUsername))
		{
			// todo bre
			throw new Exception($"User {recipientUsername} no longer has this friend request.");
		}

		if (!recipient.Friends.Any(x => x.Username == senderUsername))
		{
			// todo bre
			throw new Exception($"User {senderUsername} canceled the friend request.");
		}

		if (recipient.Friends.Count >= Globals.MaxNumberFriends)
		{
			throw new MaximumReachedException("Max number of friends reached.");
		}
	}

	public void ValidCancelFriendRequest(User user, User userToCancel)
	{
		var usernameToCancel = userToCancel.Username;
		if (!user.Friends.Any(x => x.Username == usernameToCancel))
		{
			// shouldn't be possible but just in case
			// todo bre
			throw new Exception($"Request for {usernameToCancel} already canceled"); // todo wording
		}

		if (!userToCancel.FriendRequests.Any(x => x.Username == user.Username))
		{
			// todo bre
			throw new Exception($"Friend request for {usernameToCancel} no longer present");
		}
	}

	public void ValidDeclineFriendRequest(User user, User userToDecline)
	{
		var usernameToDecline = userToDecline.Username;
		var username = user.Username;
		if (!user.FriendRequests.Any(x => x.Username == usernameToDecline))
		{
			// shouldn't be possible but just in case
			// todo bre
			throw new Exception($"Request for {usernameToDecline} already no longer present"); // todo wording
		}

		if (!userToDecline.Friends.Any(x => x.Username == username))
		{
			// todo bre
			throw new Exception($"Friend {username} no longer present");
		}
	}

	public void ValidRemoveFriend(User user, User userToRemove)
	{
		if (!user.Friends.Any(x => x.Username == userToRemove.Username))
		{
			// todo bre
			throw new Exception($"User {user.Username} no longer has {userToRemove.Username} as a friend");
		}
	}

	public void ValidBlockFriend(User user, User userToBlock)
	{
		var username = user.Username;
		var userToBlockUsername = userToBlock.Username;
		if (user.Blocked.Count >= Globals.MaxBlocked)
		{
			// todo bre
			throw new Exception($"User {username} has exceeded blocked limit");
		}

		if (user.Blocked.Any(x => x.Username == userToBlockUsername))
		{
			throw new Exception($"User {username} already blocking {userToBlockUsername}");
		}
	}

	public void ValidSendFriendRequest(User sender, User recipient)
	{
		var senderUsername = sender.Username;
		var recipientUsername = recipient.Username;
		if (sender.Friends.Count >= Globals.MaxNumberFriends)
		{
			// todo bre
			throw new Exception("Max number of friends would be exceeded");
		}

		if (recipient.UserPreferences.PrivateAccount || recipient.Blocked.Any(x => x.Username == senderUsername))
		{
			throw new Exception($"Unable to add {recipientUsername}");
		}

		if (recipient.FriendRequests.Count >= Globals.MaxFriendRequests)
		{
			throw new Exception($"{recipientUsername} has too many pending requests");
		}

		if (sender.Friends.Any(x => x.Username == recipientUsername && !x.Confirmed))
		{
			throw new Exception("Request already sent");
		}

		if (sender.Friends.Any(x => x.Username == recipientUsername && x.Confirmed))
		{
			throw new Exception($"You are already friends with {recipientUsername}");
		}

		if (sender.FriendRequests.Any(x => x.Username == recipientUsername))
		{
			throw new Exception("This user has already sent you a friend request");
		}

		if (senderUsername.Equals(recipientUsername, StringComparison.InvariantCultureIgnoreCase))
		{
			throw new Exception("Cannot send a friend request to yourself");
		}
	}

	public void ValidUpdateOwnedExercise(UpdateExerciseRequest request, User activeUser)
	{
		var oldExercise = activeUser.Exercises.FirstOrDefault(x => x.Id == request.ExerciseId);
		if (oldExercise == null)
		{
			throw new Exception($"No exercise found with id {request.ExerciseId}");
		}

		var oldExerciseName = oldExercise.ExerciseName;
		var exerciseNames = activeUser.Exercises.Select(x => x.ExerciseName);
		if (!request.Focuses.Any())
		{
			throw new Exception("Must have at least one focus");
		}

		if (request.ExerciseName != oldExerciseName && exerciseNames.Contains(request.ExerciseName))
		{
			// compare old name since user might not have changed name and otherwise would always get error saying exercise already exists
			throw new Exception("Exercise name already exists");
		}
	}

	public void ValidNewExercise(CreateExerciseRequest request, User user)
	{
		var exerciseNames = user.Exercises.Select(x => x.ExerciseName);
		if (!request.Focuses.Any())
		{
			// todo bre
			throw new Exception("Must have at least one focus");
		}

		if (exerciseNames.Any(x => x == request.ExerciseName))
		{
			throw new Exception("Exercise name already exists");
		}

		if (user.PremiumToken == null && user.Exercises.Count >= Globals.MaxFreeExercises)
		{
			throw new Exception("Max exercise limit reached");
		}

		if (user.PremiumToken != null && user.Exercises.Count >= Globals.MaxPremiumExercises)
		{
			throw new Exception("Max exercise limit reached");
		}
	}

	public void ValidRestartStatistics(User user, string workoutId)
	{
		if (!user.Workouts.Any(x => x.WorkoutId == workoutId))
		{
			// todo bre
			throw new Exception($"Workout not found on user: {workoutId}");
		}
	}
}