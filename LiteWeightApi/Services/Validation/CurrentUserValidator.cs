using LiteWeightApi.Domain;
using LiteWeightApi.Domain.Users;
using LiteWeightApi.Errors.Exceptions;

namespace LiteWeightApi.Services.Validation;

public interface ICurrentUserValidator
{
	Task ValidCreateUser(string username);

	void ValidSetCurrentWorkout(User user, string workoutId);
	// todo rest of these
}

public class CurrentUserValidator : ICurrentUserValidator
{
	private readonly IRepository _repository;

	public CurrentUserValidator(IRepository repository)
	{
		_repository = repository;
	}

	public async Task ValidCreateUser(string username)
	{
		var user = await _repository.GetUserByUsername(username);
		if (user != null)
		{
			throw new AlreadyExistsException("User already exists with this username");
		}
	}

	public void ValidSetCurrentWorkout(User user, string workoutId)
	{
		if (user.Workouts.All(x => x.WorkoutId != workoutId))
		{
			throw new WorkoutNotFoundException($"{workoutId} does not exist for the current user.");
		}
	}
}