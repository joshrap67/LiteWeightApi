using LiteWeightAPI.Domain;
using LiteWeightAPI.Errors.Exceptions;

namespace LiteWeightAPI.Services.Validation;

public interface ICurrentUserValidator
{
	Task ValidCreateUser(string username);
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
}