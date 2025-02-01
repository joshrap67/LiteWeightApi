namespace LiteWeightAPI.Commands.Users.AcceptFriendRequest;

public class AcceptFriendRequest : ICommand<bool>
{
	public required string InitiatorUserId { get; set; }
	public required string AcceptedUserId { get; set; }
}