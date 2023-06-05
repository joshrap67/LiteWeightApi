using LiteWeightAPI.Domain;

namespace LiteWeightAPI.Commands.Self.SetSettings;

public class SetSettingsHandler : ICommandHandler<SetSettings, bool>
{
	private readonly IRepository _repository;

	public SetSettingsHandler(IRepository repository)
	{
		_repository = repository;
	}

	public async Task<bool> HandleAsync(SetSettings command)
	{
		var user = await _repository.GetUser(command.UserId);
		user.Settings.Update(command.PrivateAccount, command.UpdateDefaultWeightOnSave,
			command.UpdateDefaultWeightOnRestart, command.MetricUnits);

		await _repository.PutUser(user);

		return true;
	}
}