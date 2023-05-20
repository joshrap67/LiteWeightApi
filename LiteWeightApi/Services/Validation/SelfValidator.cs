using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;

namespace LiteWeightAPI.Services.Validation;

public interface ISelfValidator
{
	Task ValidCreateSelf(string username, string email);
	void ValidSetCurrentWorkout(User user, string workoutId);
}

public class SelfValidator : ISelfValidator
{
	private readonly IRepository _repository;

	public SelfValidator(IRepository repository)
	{
		_repository = repository;
	}

	public async Task ValidCreateSelf(string username, string email)
	{
		var user = await _repository.GetUserByUsername(username);
		if (user != null)
		{
			throw new AlreadyExistsException("User already exists with this username");
		}
		
		// todo test
		var userByEmail = await _repository.GetUserByEmail(username);
		if (userByEmail != null)
		{
			throw new AlreadyExistsException("There is already an account associated with this email");
		}
	}

	public void ValidSetCurrentWorkout(User user, string workoutId)
	{
		if (user.Workouts.All(x => x.WorkoutId != workoutId))
		{
			throw new WorkoutNotFoundException($"{workoutId} does not exist for the current user");
		}
	}
}