namespace LiteWeightAPI.Commands.Users.RemoveFriend;

public class RemoveFriend : ICommand<bool>
{
	public string InitiatorUserId { get; set; }
	public string RemovedUserId { get; set; }
}