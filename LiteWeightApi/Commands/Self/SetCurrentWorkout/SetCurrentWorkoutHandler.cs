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

		if (command.WorkoutId != null && user.Workouts.All(x => x.WorkoutId != command.WorkoutId))
		{
			throw new WorkoutNotFoundException($"{command.WorkoutId} does not exist for the authenticated user");
		}

		if (command.WorkoutId != null)
		{
			var workoutInfo = user.Workouts.First(x => x.WorkoutId == command.WorkoutId);
			workoutInfo.LastSetAsCurrentUtc = _clock.GetCurrentInstant();
		}

		user.CurrentWorkoutId = command.WorkoutId;

		await _repository.PutUser(user);

		return true;
	}
}