using LiteWeightAPI.Domain;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;

namespace LiteWeightAPI.Commands.Exercises.UpdateExercise;

public class UpdateExerciseHandler : ICommandHandler<UpdateExercise, bool>
{
	private readonly IRepository _repository;

	public UpdateExerciseHandler(IRepository repository)
	{
		_repository = repository;
	}

	public async Task<bool> HandleAsync(UpdateExercise command)
	{
		var user = await _repository.GetUser(command.UserId);

		var oldExercise = user.Exercises.FirstOrDefault(x => x.Id == command.ExerciseId);
		if (oldExercise == null)
		{
			throw new ResourceNotFoundException("Exercise");
		}

		var oldExerciseName = oldExercise.Name;
		var exerciseNames = user.Exercises.Select(x => x.Name).ToHashSet();

		if (command.Name != oldExerciseName && exerciseNames.Contains(command.Name))
		{
			// compare old name since user might not have changed name and otherwise would always get error saying exercise already exists
			throw new AlreadyExistsException("Exercise name already exists");
		}

		var ownedExercise = user.Exercises.First(x => x.Id == command.ExerciseId);
		ownedExercise.Update(command.Name, command.DefaultWeight, command.DefaultSets, command.DefaultReps,
			command.DefaultDetails, command.VideoUrl, command.Focuses);

		return true;
	}
}