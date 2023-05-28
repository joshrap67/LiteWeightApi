namespace LiteWeightAPI.Commands.Users.AcceptFriendRequest;

public class AcceptFriendRequest : ICommand<bool>
{
	public string InitiatorUserId { get; set; }
	public string AcceptedUserId { get; set; }
}