using LiteWeightAPI.Api.Self.Responses;

namespace LiteWeightAPI.Commands.Users.SendFriendRequest;

public class SendFriendRequest : ICommand<FriendResponse>
{
	public string SenderId { get; init; }
	public string RecipientId { get; init; }
}