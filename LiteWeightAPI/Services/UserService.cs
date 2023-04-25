using System.Text.Json;
using AutoMapper;
using LiteWeightAPI.Api.Users.Requests;
using LiteWeightAPI.Api.Users.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Imports;
using LiteWeightAPI.Services.Notifications;
using LiteWeightAPI.Services.Notifications.Responses;
using LiteWeightAPI.Services.Validation;
using NodaTime;

namespace LiteWeightAPI.Services;

public interface IUserService
{
	Task<UserResponse> GetUser(string userId);
	Task<UserResponse> CreateUser(string username);
	Task UpdateIcon(UpdateIconRequest request, string userId);
	Task SetPushEndpoint(string token, string userId);
	Task DeletePushEndpoint(string userId);
	Task<FriendResponse> SendFriendRequest(string recipientUsername, string userId);
	Task AcceptFriendRequest(string usernameToAccept, string userId);
	Task CancelFriendRequest(string usernameToCancel, string userId);
	Task DeclineFriendRequest(string usernameToDecline, string userId);
	Task RemoveFriend(string usernameToRemove, string userId);
	Task<BlockedUserResponse> BlockUser(string usernameToBlock, string userId);
	Task SetAllFriendRequestsSeen(string userId);
	Task SetUserPreferences(UserPreferencesResponse request, string userId);
	Task<OwnedExerciseResponse> CreateExercise(CreateExerciseRequest request, string userId);
	Task UpdateExercise(UpdateExerciseRequest request, string userId);
	Task DeleteExercise(string exerciseId, string userId);
	Task<UserResponse> RestartStatistics(string workoutId, string userId);
	Task SetCurrentWorkout(string workoutId, string userId);
	Task SetAllReceivedWorkoutsSeen(string userId);
	Task SetReceivedWorkoutSeen(string workoutId, string userId);
	Task DeleteUser(string userId);
}

public class UserService : IUserService
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;
	private readonly IStorageService _storageService;
	private readonly ISnsService _snsService;
	private readonly IClock _clock;
	private readonly ICognitoService _cognitoService;
	private readonly IUserValidator _userValidator;

	public UserService(IRepository repository, IMapper mapper, IStorageService storageService,
		ISnsService snsService, IClock clock, ICognitoService cognitoService, IUserValidator userValidator)
	{
		_repository = repository;
		_mapper = mapper;
		_storageService = storageService;
		_snsService = snsService;
		_clock = clock;
		_cognitoService = cognitoService;
		_userValidator = userValidator;
	}

	public async Task<UserResponse> GetUser(string userId)
	{
		var user = await _repository.GetUser(userId);
		var retVal = _mapper.Map<UserResponse>(user);
		return retVal;
	}

	public async Task<UserResponse> CreateUser(string username)
	{
		await _userValidator.ValidCreateUser(username);
		// whenever a user is created, give them a unique UUID file path that will always get updated
		var fileName = await _storageService.UploadDefaultImage();

		var userPreferences = new UserPreferences
		{
			MetricUnits = false,
			PrivateAccount = false,
			UpdateDefaultWeightOnRestart = true,
			UpdateDefaultWeightOnSave = true
		};
		var user = new User
		{
			Icon = fileName,
			Username = username,
			UserPreferences = userPreferences,
			Exercises = Defaults.GetDefaultExercises()
		};
		var createdUser = await _repository.CreateUser(user);
		var retVal = _mapper.Map<UserResponse>(createdUser);
		return retVal;
	}

	public async Task UpdateIcon(UpdateIconRequest request, string userId)
	{
		var user = await _repository.GetUser(userId);
		await _storageService.UploadImage(request.ImageData, user.Icon);
	}

	public async Task SetPushEndpoint(string token, string userId)
	{
		var user = await _repository.GetUser(userId);
		var endpointArn = await _snsService.RegisterTokenForPushPlatform(token, user.PushEndpointArn, user.Username);
		user.PushEndpointArn = endpointArn;

		await _repository.PutUser(user);
	}

	public async Task DeletePushEndpoint(string userId)
	{
		var user = await _repository.GetUser(userId);
		await _snsService.DeletePushEndpoint(user.PushEndpointArn);

		user.PushEndpointArn = null;
		await _repository.PutUser(user);
	}

	public async Task<FriendResponse> SendFriendRequest(string recipientUsername, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var recipientUser = await _repository.GetUser(recipientUsername);
		_userValidator.ValidSendFriendRequest(activeUser, recipientUser);

		var friendToAdd = new Friend { Icon = recipientUser.Icon };
		var now = _clock.GetCurrentInstant().ToString();
		var friendRequest = new FriendRequest
		{
			Icon = activeUser.Icon,
			RequestTimeStamp = now
		};
		activeUser.Friends.Add(friendToAdd);
		recipientUser.FriendRequests.Add(friendRequest);

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { activeUser, recipientUser });

		// if this succeeds, go ahead and send a notification to user that received the request
		var friendRequestResponse = new NewFriendRequestResponse
		{
			Icon = activeUser.Icon,
			Username = userId,
			RequestTimeStamp = now
		}; // todo i feel like there should be a push notification service. that way it can be reused in the few cases where that happens
		await _snsService.SendPushNotification(recipientUser.PushEndpointArn,
			new NotificationData
			{
				Action = SnsService.FriendRequestAction,
				JsonPayload = JsonSerializer.Serialize(friendRequestResponse)
			});

		var friendResponse = _mapper.Map<FriendResponse>(friendToAdd);
		friendResponse.Username = recipientUsername;
		return friendResponse;
	}

	public async Task AcceptFriendRequest(string usernameToAccept, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var userToAccept = await _repository.GetUser(usernameToAccept);
		_userValidator.ValidAcceptFriendRequest(userToAccept, activeUser);

		// remove request from active user and add the new friend
		var newFriend = new Friend { Icon = userToAccept.Icon, Confirmed = true, Username = usernameToAccept };
		activeUser.FriendRequests.RemoveAll(x => x.Username == usernameToAccept);
		activeUser.FriendRequests.RemoveAll(x => x.Username == usernameToAccept);
		activeUser.Friends.Add(newFriend);
		// update friend to accepted for the user who sent the request
		userToAccept.Friends.First(x => x.Username == usernameToAccept).Confirmed = true;

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { activeUser, userToAccept });

		// send a notification to indicate the request was accepted
		await _snsService.SendPushNotification(userToAccept.PushEndpointArn,
			new NotificationData
			{
				Action = SnsService.AcceptedFriendRequestAction,
				JsonPayload = JsonSerializer.Serialize(new AcceptedFriendRequestResponse { Username = userId })
			});
	}

	public async Task CancelFriendRequest(string usernameToCancel, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var userToCancel = await _repository.GetUser(usernameToCancel);
		_userValidator.ValidCancelFriendRequest(activeUser, userToCancel);

		// for user cancelling, remove the (unconfirmed) user from their friends mapping
		activeUser.Friends.RemoveAll(x => x.Username == usernameToCancel);
		// for canceled user, remove the friend request
		userToCancel.FriendRequests.RemoveAll(x => x.Username == userId);

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { activeUser, userToCancel });

		// send a notification to indicate the request was canceled
		await _snsService.SendPushNotification(userToCancel.PushEndpointArn,
			new NotificationData
			{
				Action = SnsService.CanceledFriendRequestAction,
				JsonPayload = JsonSerializer.Serialize(new CanceledFriendRequestResponse { Username = userId })
			});
	}

	public async Task DeclineFriendRequest(string usernameToDecline, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var userToDecline = await _repository.GetUser(usernameToDecline);
		_userValidator.ValidDeclineFriendRequest(activeUser, userToDecline);

		// for active user, remove the friend request
		activeUser.FriendRequests.RemoveAll(x => x.Username == usernameToDecline);
		// for user who was declined, remove the (unconfirmed) user from their friends mapping
		userToDecline.Friends.RemoveAll(x => x.Username == userId);

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { activeUser, userToDecline });

		// if this succeeds, go ahead and send a notification to indicate the request was declined
		await _snsService.SendPushNotification(userToDecline.PushEndpointArn,
			new NotificationData
			{
				Action = SnsService.DeclinedFriendRequestAction,
				JsonPayload = JsonSerializer.Serialize(new DeclinedFriendRequestResponse { Username = userId })
			});
	}

	public async Task RemoveFriend(string usernameToRemove, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var userToRemove = await _repository.GetUser(usernameToRemove);
		_userValidator.ValidRemoveFriend(activeUser, userToRemove);

		activeUser.Friends.RemoveAll(x => x.Username == usernameToRemove);
		userToRemove.Friends.RemoveAll(x => x.Username == userId);

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { activeUser, userToRemove });

		// send a notification to indicate the user has been removed as a friend
		await _snsService.SendPushNotification(userToRemove.PushEndpointArn,
			new NotificationData
			{
				Action = SnsService.RemovedAsFriendAction,
				JsonPayload = JsonSerializer.Serialize(new RemovedAsFriendResponse { Username = userId })
			});
	}

	public async Task<BlockedUserResponse> BlockUser(string usernameToBlock, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var userToBlock = await _repository.GetUser(usernameToBlock);
		_userValidator.ValidBlockFriend(activeUser, userToBlock);
		if (activeUser.Blocked.Any(x => x.Username == usernameToBlock))
		{
			return new BlockedUserResponse { Icon = userToBlock.Icon, Username = usernameToBlock };
		}

		// this technically is not transactional, but i don't feel like duplicating the code
		if (activeUser.FriendRequests.Any(x => x.Username == usernameToBlock))
		{
			// to-be-blocked user sent the active user a friend request. decline the request
			await DeclineFriendRequest(usernameToBlock, userId);
		}
		else if (activeUser.Friends.Any(x => x.Username == usernameToBlock && x.Confirmed))
		{
			// both the active user and to-be-blocked user are friends. Remove them both as friends
			await RemoveFriend(usernameToBlock, userId);
		}
		else if (activeUser.Friends.Any(x => x.Username == usernameToBlock && !x.Confirmed))
		{
			// active user is sending a friend request to the to-be-blocked user. cancel the request
			await CancelFriendRequest(usernameToBlock, userId);
		}

		// note that any notifications are taken care of by the managers above, an exception would have been thrown if something went wrong

		activeUser.Blocked.Add(new Blocked { Username = usernameToBlock, Icon = userToBlock.Icon });
		await _repository.PutUser(activeUser);

		return new BlockedUserResponse { Icon = userToBlock.Icon, Username = usernameToBlock };
	}

	public async Task SetAllFriendRequestsSeen(string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		foreach (var friendRequest in activeUser.FriendRequests)
		{
			friendRequest.Seen = true;
		}

		await _repository.PutUser(activeUser);
	}

	public async Task SetUserPreferences(UserPreferencesResponse request, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var userPreferences = _mapper.Map<UserPreferences>(request);
		activeUser.UserPreferences = userPreferences;

		await _repository.PutUser(activeUser);
	}

	public async Task<OwnedExerciseResponse> CreateExercise(CreateExerciseRequest request, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		_userValidator.ValidNewExercise(request, activeUser);

		var newExercise = _mapper.Map<OwnedExercise>(request);
		var id = Guid.NewGuid().ToString();
		newExercise.Id = id;
		activeUser.Exercises.Add(newExercise);

		await _repository.PutUser(activeUser);

		var response = _mapper.Map<OwnedExerciseResponse>(newExercise);
		response.Id = id;
		return response;
	}

	public async Task UpdateExercise(UpdateExerciseRequest request, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		_userValidator.ValidUpdateOwnedExercise(request, activeUser);

		var ownedExercise = activeUser.Exercises.First(x => x.Id == request.ExerciseId);
		ownedExercise.Update(request.ExerciseName, request.DefaultWeight, request.DefaultSets, request.DefaultReps,
			request.DefaultDetails, request.VideoUrl);

		await _repository.PutUser(activeUser);
	}

	public async Task DeleteExercise(string exerciseId, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var ownedExercise = activeUser.Exercises.FirstOrDefault(x => x.Id == exerciseId);
		if (ownedExercise == null)
		{
			return;
		}

		activeUser.Exercises.RemoveAll(x => x.Id == exerciseId); // todo remove all throws exception...
		await _repository.PutUser(activeUser);
	}

	public async Task<UserResponse> RestartStatistics(string workoutId, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		_userValidator.ValidRestartStatistics(activeUser, workoutId);

		var workoutMeta = activeUser.Workouts.First(x => x.WorkoutId == workoutId);
		workoutMeta.TimesCompleted = 0;
		workoutMeta.AverageExercisesCompleted = 0.0;
		workoutMeta.TotalExercisesSum = 0;

		await _repository.PutUser(activeUser);

		return _mapper.Map<UserResponse>(activeUser);
	}

	public async Task SetCurrentWorkout(string workoutId, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		// todo check if workout exists?
		activeUser.CurrentWorkout = workoutId;
		await _repository.PutUser(activeUser);
	}

	public async Task SetAllReceivedWorkoutsSeen(string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		foreach (var sharedWorkoutMeta in activeUser.ReceivedWorkouts)
		{
			sharedWorkoutMeta.Seen = true;
		}

		await _repository.PutUser(activeUser);
	}

	public async Task SetReceivedWorkoutSeen(string workoutId, string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		var sharedWorkoutMeta = activeUser.ReceivedWorkouts.FirstOrDefault(x => x.WorkoutId == workoutId);
		if (sharedWorkoutMeta == null)
		{
			return;
		}

		sharedWorkoutMeta.Seen = true;
		await _repository.PutUser(activeUser);
	}

	public async Task DeleteUser(string userId)
	{
		var activeUser = await _repository.GetUser(userId);
		await _storageService.DeleteImage(activeUser.Icon);
		var workoutsToDelete = activeUser.Workouts.Select(x => x.WorkoutId).ToList();
		var sharedWorkoutsToDelete = activeUser.ReceivedWorkouts.Select(x => x.WorkoutId).ToList();
		var usersWhoSentFriendRequests = activeUser.FriendRequests.Select(x => x.Username).ToList();
		var usersWhoHavePendingFriendRequests = activeUser.Friends
			.Where(x => !x.Confirmed)
			.Select(x => x.Username)
			.ToList();
		var usersWhoAreFriends = activeUser.Friends.Select(x => x.Username).ToList();

		// can't really rely on transactions here
		foreach (var workoutId in workoutsToDelete)
		{
			await _repository.DeleteWorkout(workoutId);
		}

		foreach (var workoutId in sharedWorkoutsToDelete)
		{
			await _repository.DeleteSharedWorkout(workoutId);
		}

		foreach (var username in usersWhoSentFriendRequests)
		{
			var otherUser = await _repository.GetUser(username);
			otherUser.Friends.RemoveAll(x => x.Username == userId);
			await _repository.PutUser(otherUser);
		}

		foreach (var username in usersWhoHavePendingFriendRequests)
		{
			var otherUser = await _repository.GetUser(username);
			otherUser.FriendRequests.RemoveAll(x => x.Username == userId);
			await _repository.PutUser(otherUser);
		}

		foreach (var username in usersWhoAreFriends)
		{
			var otherUser = await _repository.GetUser(username);
			otherUser.Friends.RemoveAll(x => x.Username == userId);
			await _repository.PutUser(otherUser);
		}

		await _snsService.DeletePushEndpoint(activeUser.PushEndpointArn);
		await _repository.DeleteUser(userId);
		await _cognitoService.DeleteUser(userId);

		// rabbitmq to retry?
	}
}