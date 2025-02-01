namespace LiteWeightAPI.Commands.Users.DeclineFriendRequest;

public class DeclineFriendRequest : ICommand<bool>
{
	public required string InitiatorUserId { get; set; }
	public required string UserIdToDecline { get; set; }
}