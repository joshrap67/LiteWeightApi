namespace LiteWeightAPI.Commands.Self.SetAllFriendRequestsSeen;

public class SetAllFriendRequestsSeen : ICommand<bool>
{
	public string UserId { get; set; }
}