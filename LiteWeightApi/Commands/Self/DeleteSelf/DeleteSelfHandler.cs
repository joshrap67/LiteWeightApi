using LiteWeightAPI.Domain;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Services;

namespace LiteWeightAPI.Commands.Self.DeleteSelf;

public class DeleteSelfHandler : ICommandHandler<DeleteSelf, bool>
{
	private readonly IRepository _repository;
	private readonly IStorageService _storageService;
	private readonly IFirebaseAuthService _firebaseAuthService;

	public DeleteSelfHandler(IRepository repository, IStorageService storageService,
		IFirebaseAuthService firebaseAuthService)
	{
		_repository = repository;
		_storageService = storageService;
		_firebaseAuthService = firebaseAuthService;
	}

	public async Task<bool> HandleAsync(DeleteSelf command)
	{
		var user = await _repository.GetUser(command.UserId);
		if (user == null)
		{
			// todo now that there is no validator class just pass in message? Or make an optional param on get method to throw?
			throw new ResourceNotFoundException("Self");
		}

		await _storageService.DeleteProfilePicture(user.ProfilePicture);
		var workoutsToDelete = user.Workouts.Select(x => x.WorkoutId).ToList();
		var sharedWorkoutsToDelete = user.ReceivedWorkouts.Select(x => x.SharedWorkoutId).ToList();

		var usersWhoSentFriendRequests = user.FriendRequests.Select(x => x.UserId).ToList();
		var usersWhoAreFriends = user.Friends
			.Where(x => x.Confirmed)
			.Select(x => x.UserId)
			.ToList();
		var usersWhoReceivedFriendRequests = user.Friends
			.Where(x => !x.Confirmed)
			.Select(x => x.UserId).ToList();

		// can't really rely on transactions here
		foreach (var workoutId in workoutsToDelete)
		{
			await _repository.DeleteWorkout(workoutId);
		}

		foreach (var workoutId in sharedWorkoutsToDelete)
		{
			await _repository.DeleteSharedWorkout(workoutId);
		}

		foreach (var otherUserId in usersWhoSentFriendRequests)
		{
			var otherUser = await _repository.GetUser(otherUserId);
			otherUser.Friends.RemoveAll(x => x.UserId == command.UserId);
			await _repository.PutUser(otherUser);
		}

		foreach (var otherUserId in usersWhoAreFriends)
		{
			var otherUser = await _repository.GetUser(otherUserId);
			otherUser.Friends.RemoveAll(x => x.UserId == command.UserId);
			await _repository.PutUser(otherUser);
		}

		foreach (var otherUserId in usersWhoReceivedFriendRequests)
		{
			var otherUser = await _repository.GetUser(otherUserId);
			otherUser.FriendRequests.RemoveAll(x => x.UserId == command.UserId);
			await _repository.PutUser(otherUser);
		}

		// todo send push notifications for removed/canceled? Might be a bit overkill
		await _repository.DeleteUser(command.UserId);
		await _firebaseAuthService.DeleteUser(command.UserId);

		// rabbitmq to retry?
		return true;
	}
}