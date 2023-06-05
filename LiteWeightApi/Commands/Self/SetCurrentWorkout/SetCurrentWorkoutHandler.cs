using LiteWeightAPI.Domain;
using LiteWeightAPI.Errors.Exceptions;
using NodaTime;

namespace LiteWeightAPI.Commands.Self.SetCurrentWorkout;

public class SetCurrentWorkoutHandler : ICommandHandler<SetCurrentWorkout, bool>
{
	private readonly IRepository _repository;
	private readonly IClock _clock;

	public SetCurrentWorkoutHandler(IRepository repository, IClock clock)
	{
		_repository = repository;
		_clock = clock;
	}

	public async Task<bool> HandleAsync(SetCurrentWorkout command)
	{
		var user = await _repository.GetUser(command.UserId);

		if (command.CurrentWorkoutId != null && user.Workouts.All(x => x.WorkoutId != command.CurrentWorkoutId))
		{
			throw new WorkoutNotFoundException($"{command.CurrentWorkoutId} does not exist for the authenticated user");
		}

		if (command.CurrentWorkoutId != null)
		{
			var workoutInfo = user.Workouts.First(x => x.WorkoutId == command.CurrentWorkoutId);
			workoutInfo.LastSetAsCurrentUtc = _clock.GetCurrentInstant();
		}

		user.CurrentWorkoutId = command.CurrentWorkoutId;

		await _repository.PutUser(user);

		return true;
	}
}