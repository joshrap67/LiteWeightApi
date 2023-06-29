using LiteWeightAPI.Domain;

namespace LiteWeightAPI.Commands.Self.SetReceivedWorkoutSeen;

public class SetReceivedWorkoutSeenHandler : ICommandHandler<SetReceivedWorkoutSeen, bool>
{
	private readonly IRepository _repository;

	public SetReceivedWorkoutSeenHandler(IRepository repository)
	{
		_repository = repository;
	}

	public async Task<bool> HandleAsync(SetReceivedWorkoutSeen command)
	{
		var user = await _repository.GetUser(command.UserId);
		var receivedWorkoutInfo = user.ReceivedWorkouts.FirstOrDefault(x => x.ReceivedWorkoutId == command.ReceivedWorkoutId);
		if (receivedWorkoutInfo == null)
		{
			return false;
		}

		receivedWorkoutInfo.Seen = true;
		await _repository.PutUser(user);

		return true;
	}
}