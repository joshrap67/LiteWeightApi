using LiteWeightAPI.Domain;

namespace LiteWeightAPI.Commands.Self.SetFirebaseToken;

public class SetFirebaseTokenHandler : ICommandHandler<SetFirebaseToken, bool>
{
	private readonly IRepository _repository;

	public SetFirebaseTokenHandler(IRepository repository)
	{
		_repository = repository;
	}

	public async Task<bool> HandleAsync(SetFirebaseToken command)
	{
		var user = await _repository.GetUser(command.UserId);
		user.FirebaseMessagingToken = command.Token;

		await _repository.PutUser(user);

		return true;
	}
}