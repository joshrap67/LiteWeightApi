namespace LiteWeightAPI.Commands.Self.SetAllFriendRequestsSeen;

public class SetAllFriendRequestsSeen : ICommand<bool>
{
	public required string UserId { get; init; }
}