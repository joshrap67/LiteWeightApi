using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Utils;
using NodaTime;

namespace LiteWeightAPI.Commands.Workouts.DeleteWorkoutAndSetCurrent;

public class DeleteWorkoutAndSetCurrentHandler : ICommandHandler<DeleteWorkoutAndSetCurrent, bool>
{
	private readonly IRepository _repository;
	private readonly IClock _clock;

	public DeleteWorkoutAndSetCurrentHandler(IRepository repository, IClock clock)
	{
		_repository = repository;
		_clock = clock;
	}

	public async Task<bool> HandleAsync(DeleteWorkoutAndSetCurrent command)
	{
		var workoutToDelete = await _repository.GetWorkout(command.WorkoutToDeleteId);
		var user = await _repository.GetUser(command.UserId);

		CommonValidator.WorkoutExists(workoutToDelete);
		CommonValidator.EnsureWorkoutOwnership(user.Id, workoutToDelete);

		user.Workouts.RemoveAll(x => x.WorkoutId == command.WorkoutToDeleteId);
		foreach (var ownedExercise in user.Exercises)
		{
			var ownedExerciseWorkout =
				ownedExercise.Workouts.FirstOrDefault(x => x.WorkoutId == command.WorkoutToDeleteId);
			if (ownedExerciseWorkout != null)
			{
				ownedExercise.Workouts.Remove(ownedExerciseWorkout);
			}
		}

		// set current workout
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

		await _repository.ExecuteBatchWrite(
			workoutsToDelete: new List<Workout> { workoutToDelete },
			usersToPut: new List<User> { user }
		);

		return true;
	}
}