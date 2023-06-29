using LiteWeightAPI.Domain;

namespace LiteWeightAPI.Commands.Self.SetAllReceivedWorkoutsSeen;

public class SetAllReceivedWorkoutsSeenHandler : ICommandHandler<SetAllReceivedWorkoutsSeen, bool>
{
	private readonly IRepository _repository;

	public SetAllReceivedWorkoutsSeenHandler(IRepository repository)
	{
		_repository = repository;
	}
	
	public async Task<bool> HandleAsync(SetAllReceivedWorkoutsSeen command)
	{
		var user = await _repository.GetUser(command.UserId);
		foreach (var receivedWorkoutInfo in user.ReceivedWorkouts)
		{
			receivedWorkoutInfo.Seen = true;
		}

		await _repository.PutUser(user);

		return true;
	}
}