namespace LiteWeightAPI.Commands.Users.RemoveFriend;

public class RemoveFriend : ICommand<bool>
{
	public required string InitiatorUserId { get; set; }
	public required string RemovedUserId { get; set; }
}