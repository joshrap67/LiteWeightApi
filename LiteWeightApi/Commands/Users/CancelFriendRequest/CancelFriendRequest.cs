namespace LiteWeightAPI.Commands.Users.CancelFriendRequest;

public class CancelFriendRequest : ICommand<bool>
{
	public required string InitiatorUserId { get; set; }
	public required string UserIdToCancel { get; set; }
}