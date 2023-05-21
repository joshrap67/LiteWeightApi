using AutoMapper;
using LiteWeightAPI.Api.Self.Requests;
using LiteWeightAPI.Api.Self.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;
using LiteWeightAPI.Services.Validation;
using NodaTime;

namespace LiteWeightAPI.Services;

public interface ISelfService
{
	Task<UserResponse> GetSelf(string userId);
	Task<UserResponse> CreateSelf(CreateUserRequest createUserRequest, string email, string firebaseId);
	Task UpdateProfilePicture(UpdateProfilePictureRequest request, string userId);
	Task LinkFirebaseToken(string token, string userId);
	Task UnlinkFirebaseToken(string userId);
	Task SetAllFriendRequestsSeen(string userId);
	Task SetUserPreferences(UserPreferencesResponse request, string userId);
	Task SetCurrentWorkout(string workoutId, string userId);
	Task SetAllReceivedWorkoutsSeen(string userId);
	Task SetReceivedWorkoutSeen(string sharedWorkoutId, string userId);
	Task DeleteUser(string userId);
}

public class SelfService : ISelfService
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;
	private readonly IStorageService _storageService;
	private readonly IFirebaseAuthService _firebaseAuthService;
	private readonly ISelfValidator _selfValidator;
	private readonly IClock _clock;

	public SelfService(IRepository repository, IMapper mapper, IStorageService storageService,
		IFirebaseAuthService firebaseAuthService, ISelfValidator selfValidator, IClock clock)
	{
		_repository = repository;
		_mapper = mapper;
		_storageService = storageService;
		_firebaseAuthService = firebaseAuthService;
		_selfValidator = selfValidator;
		_clock = clock;
	}

	public async Task<UserResponse> GetSelf(string userId)
	{
		var user = await _repository.GetUser(userId);
		if (user == null)
		{
			throw new ResourceNotFoundException("User");
		}

		var retVal = _mapper.Map<UserResponse>(user);
		return retVal;
	}

	public async Task<UserResponse> CreateSelf(CreateUserRequest request, string email, string firebaseId)
	{
		await _selfValidator.ValidCreateSelf(request.Username, email);

		// whenever a user is created, give them a unique UUID file path that will always get updated
		var fileName = Guid.NewGuid().ToString();
		if (request.ProfilePictureData != null)
		{
			await _storageService.UploadProfilePicture(request.ProfilePictureData, fileName);
		}
		else
		{
			await _storageService.UploadDefaultProfilePicture(fileName);
		}

		var userPreferences = new UserPreferences
		{
			MetricUnits = request.MetricUnits,
			PrivateAccount = false,
			UpdateDefaultWeightOnRestart = true,
			UpdateDefaultWeightOnSave = true
		};
		var user = new User
		{
			Id = firebaseId,
			Email = email,
			ProfilePicture = fileName,
			Username = request.Username,
			UserPreferences = userPreferences,
			Exercises = Defaults.GetDefaultExercises()
		};
		await _repository.CreateUser(user);

		var retVal = _mapper.Map<UserResponse>(user);
		return retVal;
	}

	public async Task UpdateProfilePicture(UpdateProfilePictureRequest request, string userId)
	{
		var user = await _repository.GetUser(userId);
		await _storageService.UploadProfilePicture(request.ImageData, user.ProfilePicture);
	}

	public async Task LinkFirebaseToken(string token, string userId)
	{
		var user = await _repository.GetUser(userId);
		user.FirebaseMessagingToken = token;

		await _repository.PutUser(user);
	}

	public async Task UnlinkFirebaseToken(string userId)
	{
		var user = await _repository.GetUser(userId);
		user.FirebaseMessagingToken = null;

		await _repository.PutUser(user);
	}

	public async Task SetAllFriendRequestsSeen(string userId)
	{
		var user = await _repository.GetUser(userId);
		foreach (var friendRequest in user.FriendRequests)
		{
			friendRequest.Seen = true;
		}

		await _repository.PutUser(user);
	}

	public async Task SetUserPreferences(UserPreferencesResponse request, string userId)
	{
		var user = await _repository.GetUser(userId);
		var userPreferences = _mapper.Map<UserPreferences>(request);
		user.UserPreferences = userPreferences;

		await _repository.PutUser(user);
	}

	public async Task SetCurrentWorkout(string workoutId, string userId)
	{
		var user = await _repository.GetUser(userId);
		_selfValidator.ValidSetCurrentWorkout(user, workoutId);

		if (workoutId != null)
		{
			var workoutInfo = user.Workouts.First(x => x.WorkoutId == workoutId);
			workoutInfo.LastSetAsCurrentUtc = _clock.GetCurrentInstant();
		}

		user.CurrentWorkoutId = workoutId;

		await _repository.PutUser(user);
	}

	public async Task SetAllReceivedWorkoutsSeen(string userId)
	{
		var user = await _repository.GetUser(userId);
		foreach (var sharedWorkoutMeta in user.ReceivedWorkouts)
		{
			sharedWorkoutMeta.Seen = true;
		}

		await _repository.PutUser(user);
	}

	public async Task SetReceivedWorkoutSeen(string sharedWorkoutId, string userId)
	{
		var user = await _repository.GetUser(userId);
		var sharedWorkoutMeta = user.ReceivedWorkouts.FirstOrDefault(x => x.SharedWorkoutId == sharedWorkoutId);
		if (sharedWorkoutMeta == null)
		{
			return;
		}

		sharedWorkoutMeta.Seen = true;
		await _repository.PutUser(user);
	}

	public async Task DeleteUser(string userId)
	{
		var user = await _repository.GetUser(userId);
		if (user == null)
		{
			return;
		}

		await _storageService.DeleteProfilePicture(user.ProfilePicture);
		var workoutsToDelete = user.Workouts.Select(x => x.WorkoutId).ToList();
		var sharedWorkoutsToDelete = user.ReceivedWorkouts.Select(x => x.SharedWorkoutId).ToList();
		var usersWhoSentFriendRequests = user.FriendRequests.Select(x => x.Username).ToList();
		var usersWhoAreFriends = user.Friends.Select(x => x.UserId).ToList();

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
			otherUser.Friends.RemoveAll(x => x.UserId == userId);
			await _repository.PutUser(otherUser);
		}

		foreach (var otherUserId in usersWhoAreFriends)
		{
			var otherUser = await _repository.GetUser(otherUserId);
			otherUser.Friends.RemoveAll(x => x.UserId == userId);
			await _repository.PutUser(otherUser);
		}

		await _repository.DeleteUser(userId);
		await _firebaseAuthService.DeleteUser(userId);

		// rabbitmq to retry?
	}
}