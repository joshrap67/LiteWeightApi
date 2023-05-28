using LiteWeightAPI.Domain;

namespace LiteWeightAPI.Commands.Self.SetPreferences;

public class SetPreferencesHandler : ICommandHandler<SetPreferences, bool>
{
	private readonly IRepository _repository;

	public SetPreferencesHandler(IRepository repository)
	{
		_repository = repository;
	}

	public async Task<bool> HandleAsync(SetPreferences command)
	{
		var user = await _repository.GetUser(command.UserId);
		user.Preferences.Update(command.PrivateAccount, command.UpdateDefaultWeightOnSave,
			command.UpdateDefaultWeightOnRestart, command.MetricUnits);

		await _repository.PutUser(user);

		return true;
	}
}