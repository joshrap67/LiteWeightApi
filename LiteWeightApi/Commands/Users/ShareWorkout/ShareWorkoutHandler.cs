using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;
using LiteWeightAPI.Services;
using LiteWeightAPI.Utils;
using NodaTime;

namespace LiteWeightAPI.Commands.Users.ShareWorkout;

public class ShareWorkoutHandler : ICommandHandler<ShareWorkout, string>
{
	private readonly IRepository _repository;
	private readonly IPushNotificationService _pushNotificationService;
	private readonly IClock _clock;
	private readonly IStatisticsService _statisticsService;

	public ShareWorkoutHandler(IRepository repository, IPushNotificationService pushNotificationService, IClock clock,
		IStatisticsService statisticsService)
	{
		_repository = repository;
		_pushNotificationService = pushNotificationService;
		_clock = clock;
		_statisticsService = statisticsService;
	}

	public async Task<string> HandleAsync(ShareWorkout command)
	{
		var senderUser = await _repository.GetUser(command.SenderUserId);
		var recipientUser = await _repository.GetUser(command.RecipientUserId);
		var workoutToSend = await _repository.GetWorkout(command.WorkoutId);

		if (command.SenderUserId == command.RecipientUserId)
		{
			throw new MiscErrorException("Cannot send workout to yourself");
		}

		ValidationUtils.UserExists(recipientUser);
		ValidationUtils.ReferencedWorkoutExists(workoutToSend);
		ValidationUtils.EnsureWorkoutOwnership(command.SenderUserId, workoutToSend);

		if (recipientUser.Preferences.PrivateAccount &&
		    recipientUser.Friends.All(x => x.UserId != command.SenderUserId))
		{
			throw new ResourceNotFoundException("User");
		}

		if (recipientUser.ReceivedWorkouts.Count >= Globals.MaxReceivedWorkouts)
		{
			throw new MaxLimitException($"{command.RecipientUserId} has too many received workouts");
		}

		if (senderUser.PremiumToken == null && senderUser.WorkoutsSent >= Globals.MaxFreeWorkoutsSent)
		{
			throw new MaxLimitException("You have reached the maximum number of workouts that you can send");
		}

		var sharedWorkoutId = Guid.NewGuid().ToString();
		var sharedWorkoutInfo = new SharedWorkoutInfo
		{
			SharedWorkoutId = sharedWorkoutId,
			SenderId = command.SenderUserId,
			SenderUsername = senderUser.Username,
			SenderProfilePicture = senderUser.ProfilePicture,
			WorkoutName = workoutToSend.Name,
			SharedUtc = _clock.GetCurrentInstant(),
			TotalDays = workoutToSend.Routine.TotalNumberOfDays,
			MostFrequentFocus = _statisticsService
				.FindMostFrequentFocus(senderUser.Exercises.ToDictionary(x => x.Id, x => x), workoutToSend.Routine)
		};
		recipientUser.ReceivedWorkouts.Add(sharedWorkoutInfo);
		senderUser.WorkoutsSent++;

		var sharedWorkout = new SharedWorkout(workoutToSend, command.RecipientUserId, sharedWorkoutId, senderUser);

		await _repository.ExecuteBatchWrite(
			usersToPut: new List<User> { senderUser, recipientUser },
			sharedWorkoutsToPut: new List<SharedWorkout> { sharedWorkout }
		);

		await _pushNotificationService.SendWorkoutPushNotification(recipientUser, sharedWorkoutInfo);

		return sharedWorkoutId;
	}
}