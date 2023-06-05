using LiteWeightAPI.Domain;

namespace LiteWeightAPI.Commands.Self.SetFirebaseMessagingToken;

public class SetFirebaseMessagingTokenHandler : ICommandHandler<SetFirebaseMessagingToken, bool>
{
	private readonly IRepository _repository;

	public SetFirebaseMessagingTokenHandler(IRepository repository)
	{
		_repository = repository;
	}

	public async Task<bool> HandleAsync(SetFirebaseMessagingToken command)
	{
		var user = await _repository.GetUser(command.UserId);
		user.FirebaseMessagingToken = command.Token;

		await _repository.PutUser(user);

		return true;
	}
}