using AutoMapper;
using LiteWeightAPI.Api.CurrentUser.Requests;
using LiteWeightAPI.Api.CurrentUser.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;
using LiteWeightAPI.Services.Validation;
using NodaTime;

namespace LiteWeightAPI.Services;

public interface ICurrentUserService
{
	Task<UserResponse> GetUser(string userId);
	Task<UserResponse> CreateUser(CreateUserRequest createUserRequest, string firebaseId);
	Task UpdateIcon(UpdateIconRequest request, string userId);
	Task SetPushEndpoint(string token, string userId);
	Task DeletePushEndpoint(string userId);
	Task SetAllFriendRequestsSeen(string userId);
	Task SetUserPreferences(UserPreferencesResponse request, string userId);
	Task SetCurrentWorkout(string workoutId, string userId);
	Task SetAllReceivedWorkoutsSeen(string userId);
	Task SetReceivedWorkoutSeen(string sharedWorkoutId, string userId);
	Task DeleteUser(string userId);
}

public class CurrentUserService : ICurrentUserService
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;
	private readonly IStorageService _storageService;
	private readonly ICognitoService _cognitoService;
	private readonly ICurrentUserValidator _currentUserValidator;
	private readonly IClock _clock;

	public CurrentUserService(IRepository repository, IMapper mapper, IStorageService storageService,
		ICognitoService cognitoService, ICurrentUserValidator currentUserValidator, IClock clock)
	{
		_repository = repository;
		_mapper = mapper;
		_storageService = storageService;
		_cognitoService = cognitoService;
		_currentUserValidator = currentUserValidator;
		_clock = clock;
	}

	public async Task<UserResponse> GetUser(string userId)
	{
		var user = await _repository.GetUser(userId);
		if (user == null)
		{
			throw new ResourceNotFoundException("User");
		}

		var retVal = _mapper.Map<UserResponse>(user);
		return retVal;
	}

	public async Task<UserResponse> CreateUser(CreateUserRequest request, string firebaseId)
	{
		// todo grab email, firebase id from token
		await _currentUserValidator.ValidCreateUser(request.Username);

		// whenever a user is created, give them a unique UUID file path that will always get updated
		var fileName = await _storageService.UploadDefaultProfilePicture();
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
			Icon = fileName,
			Username = request.Username,
			UserPreferences = userPreferences,
			Exercises = Defaults.GetDefaultExercises()
		};
		await _repository.CreateUser(user);

		var retVal = _mapper.Map<UserResponse>(user);
		return retVal;
	}

	public async Task UpdateIcon(UpdateIconRequest request, string userId)
	{
		var user = await _repository.GetUser(userId);
		await _storageService.UploadProfilePicture(request.ImageData, user.Icon);
	}

	public async Task SetPushEndpoint(string token, string userId)
	{
		var user = await _repository.GetUser(userId);
		user.FirebaseMessagingToken = token;

		await _repository.PutUser(user);
	}

	public async Task DeletePushEndpoint(string userId)
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
		if (workoutId != null)
		{
			// todo check if workout exists?
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

		await _storageService.DeleteProfilePicture(user.Icon);
		var workoutsToDelete = user.Workouts.Select(x => x.WorkoutId).ToList();
		var sharedWorkoutsToDelete = user.ReceivedWorkouts.Select(x => x.SharedWorkoutId).ToList();
		var usersWhoSentFriendRequests = user.FriendRequests.Select(x => x.Username).ToList();
		var usersWhoHavePendingFriendRequests = user.Friends
			.Where(x => !x.Confirmed)
			.Select(x => x.Username)
			.ToList();
		var usersWhoAreFriends = user.Friends.Select(x => x.Username).ToList();

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

		await _repository.DeleteUser(userId);
		await _cognitoService.DeleteUser(userId);

		// rabbitmq to retry?
	}
}