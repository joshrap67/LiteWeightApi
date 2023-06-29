using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.ReceivedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Commands.ReceivedWorkouts.DeclineReceivedWorkout;

public class DeclineReceivedWorkoutHandler : ICommandHandler<DeclineReceivedWorkout, bool>
{
	private readonly IRepository _repository;

	public DeclineReceivedWorkoutHandler(IRepository repository)
	{
		_repository = repository;
	}

	public async Task<bool> HandleAsync(DeclineReceivedWorkout command)
	{
		var user = await _repository.GetUser(command.UserId);
		var workoutToDecline = await _repository.GetReceivedWorkout(command.ReceivedWorkoutId);

		ValidationUtils.ReceivedWorkoutExists(workoutToDecline);
		ValidationUtils.EnsureReceivedWorkoutOwnership(command.UserId, workoutToDecline);

		var workoutToRemove = user.ReceivedWorkouts.FirstOrDefault(x => x.ReceivedWorkoutId == command.ReceivedWorkoutId);
		user.ReceivedWorkouts.Remove(workoutToRemove);

		await _repository.ExecuteBatchWrite(
			usersToPut: new List<User> { user },
			receivedWorkoutsToDelete: new List<ReceivedWorkout> { workoutToDecline }
		);

		return true;
	}
}