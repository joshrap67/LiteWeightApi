using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Commands.SharedWorkouts.DeclineWorkout;

public class DeclineWorkoutHandler : ICommandHandler<DeclineWorkout, bool>
{
	private readonly IRepository _repository;

	public DeclineWorkoutHandler(IRepository repository)
	{
		_repository = repository;
	}

	public async Task<bool> HandleAsync(DeclineWorkout command)
	{
		var user = await _repository.GetUser(command.UserId);
		var workoutToDecline = await _repository.GetSharedWorkout(command.SharedWorkoutId);

		CommonValidator.SharedWorkoutExists(workoutToDecline);
		CommonValidator.EnsureSharedWorkoutOwnership(command.UserId, workoutToDecline);

		var workoutToRemove = user.ReceivedWorkouts.First(x => x.SharedWorkoutId == command.SharedWorkoutId);
		user.ReceivedWorkouts.Remove(workoutToRemove);

		await _repository.ExecuteBatchWrite(
			usersToPut: new List<User> { user },
			sharedWorkoutsToDelete: new List<SharedWorkout> { workoutToDecline }
		);

		return true;
	}
}