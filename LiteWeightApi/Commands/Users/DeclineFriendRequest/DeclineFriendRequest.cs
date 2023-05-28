namespace LiteWeightAPI.Commands.Users.DeclineFriendRequest;

public class DeclineFriendRequest : ICommand<bool>
{
	public string InitiatorUserId { get; set; }
	public string UserIdToDecline { get; set; }
}