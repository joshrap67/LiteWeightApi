using LiteWeightApi.Api.Exercises.Requests;
using LiteWeightApi.Domain.Users;
using LiteWeightApi.Errors.Exceptions;
using LiteWeightApi.Errors.Exceptions.BaseExceptions;
using LiteWeightApi.Imports;

namespace LiteWeightApi.Services.Validation;

public interface IExercisesValidator
{
	void ValidCreateExercise(SetExerciseRequest request, User user);
	void ValidUpdateOwnedExercise(string exerciseId, SetExerciseRequest request, User user);
}

public class ExercisesValidator : IExercisesValidator
{
	public void ValidCreateExercise(SetExerciseRequest request, User user)
	{
		var exerciseNames = user.Exercises.Select(x => x.Name);

		if (exerciseNames.Any(x => x == request.Name))
		{
			throw new AlreadyExistsException("Exercise name already exists");
		}

		if (user.PremiumToken == null && user.Exercises.Count >= Globals.MaxFreeExercises)
		{
			throw new MaxLimitException("Max exercise limit reached");
		}

		if (user.PremiumToken != null && user.Exercises.Count >= Globals.MaxPremiumExercises)
		{
			throw new MaxLimitException("Max exercise limit reached");
		}
	}

	public void ValidUpdateOwnedExercise(string exerciseId, SetExerciseRequest request, User user)
	{
		var oldExercise = user.Exercises.FirstOrDefault(x => x.Id == exerciseId);
		if (oldExercise == null)
		{
			throw new ResourceNotFoundException("Exercise");
		}

		var oldExerciseName = oldExercise.Name;
		var exerciseNames = user.Exercises.Select(x => x.Name).ToHashSet();

		if (request.Name != oldExerciseName && exerciseNames.Contains(request.Name))
		{
			// compare old name since user might not have changed name and otherwise would always get error saying exercise already exists
			throw new AlreadyExistsException("Exercise name already exists");
		}
	}
}