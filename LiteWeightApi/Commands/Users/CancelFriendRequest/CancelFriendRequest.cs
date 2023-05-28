namespace LiteWeightAPI.Commands.Users.CancelFriendRequest;

public class CancelFriendRequest : ICommand<bool>
{
	public string InitiatorUserId { get; set; }
	public string UserIdToCancel { get; set; }
}