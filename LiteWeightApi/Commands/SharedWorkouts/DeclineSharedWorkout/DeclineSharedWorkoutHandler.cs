using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Commands.SharedWorkouts.DeclineSharedWorkout;

public class DeclineSharedWorkoutHandler : ICommandHandler<DeclineSharedWorkout, bool>
{
	private readonly IRepository _repository;

	public DeclineSharedWorkoutHandler(IRepository repository)
	{
		_repository = repository;
	}

	public async Task<bool> HandleAsync(DeclineSharedWorkout command)
	{
		var user = await _repository.GetUser(command.UserId);
		var workoutToDecline = await _repository.GetSharedWorkout(command.SharedWorkoutId);

		ValidationUtils.SharedWorkoutExists(workoutToDecline);
		ValidationUtils.EnsureSharedWorkoutOwnership(command.UserId, workoutToDecline);

		var workoutToRemove = user.ReceivedWorkouts.FirstOrDefault(x => x.SharedWorkoutId == command.SharedWorkoutId);
		user.ReceivedWorkouts.Remove(workoutToRemove);

		await _repository.ExecuteBatchWrite(
			usersToPut: new List<User> { user },
			sharedWorkoutsToDelete: new List<SharedWorkout> { workoutToDecline }
		);

		return true;
	}
}