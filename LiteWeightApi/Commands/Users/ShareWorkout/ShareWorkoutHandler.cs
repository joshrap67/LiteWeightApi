using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;
using LiteWeightAPI.Services;
using LiteWeightAPI.Services.Notifications;
using LiteWeightAPI.Utils;
using NodaTime;

namespace LiteWeightAPI.Commands.Users.ShareWorkout;

public class ShareWorkoutHandler : ICommandHandler<ShareWorkout, string>
{
	private readonly IRepository _repository;
	private readonly IPushNotificationService _pushNotificationService;
	private readonly IClock _clock;

	public ShareWorkoutHandler(IRepository repository, IPushNotificationService pushNotificationService, IClock clock)
	{
		_repository = repository;
		_pushNotificationService = pushNotificationService;
		_clock = clock;
	}

	public async Task<string> HandleAsync(ShareWorkout command)
	{
		var senderUser = await _repository.GetUser(command.SenderUserId);
		var recipientUser = await _repository.GetUser(command.RecipientUserId);
		var workoutToSend = await _repository.GetWorkout(command.WorkoutId);

		CommonValidator.UserExists(recipientUser);
		CommonValidator.ReferencedWorkoutExists(workoutToSend);
		CommonValidator.EnsureWorkoutOwnership(senderUser.Id, workoutToSend);

		var senderId = senderUser.Id;

		if (recipientUser.Preferences.PrivateAccount && recipientUser.Friends.All(x => x.UserId != senderId))
		{
			throw new ResourceNotFoundException("User");
		}

		if (recipientUser.ReceivedWorkouts.Count >= Globals.MaxReceivedWorkouts)
		{
			throw new MaxLimitException($"{command.RecipientUserId} has too many received workouts");
		}

		if (senderUser.WorkoutsSent >= Globals.MaxFreeWorkoutsSent)
		{
			throw new MaxLimitException("You have reached the maximum number of workouts that you can send");
		}

		if (senderId == command.RecipientUserId)
		{
			throw new MiscErrorException("Cannot send workout to yourself");
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
			MostFrequentFocus = FindMostFrequentFocus(senderUser, workoutToSend.Routine)
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

	private static string FindMostFrequentFocus(User user, Routine routine)
	{
		var exerciseIdToExercise = user.Exercises.ToDictionary(x => x.Id, x => x);
		var focusCount = new Dictionary<string, int>();
		foreach (var week in routine.Weeks)
		{
			foreach (var day in week.Days)
			{
				foreach (var routineExercise in day.Exercises)
				{
					var exerciseId = routineExercise.ExerciseId;
					foreach (var focus in exerciseIdToExercise[exerciseId].Focuses)
					{
						if (!focusCount.ContainsKey(focus))
						{
							focusCount[focus] = 1;
						}
						else
						{
							focusCount[focus]++;
						}
					}
				}
			}
		}

		var max = focusCount.Values.Max();

		var maxFocuses = (from focus in focusCount.Keys
				let count = focusCount[focus]
				where count == max
				select focus)
			.ToList();

		var retVal = string.Join(",", maxFocuses);
		return retVal;
	}
}