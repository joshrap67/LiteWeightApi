using LiteWeightAPI.Api.Self.Responses;

namespace LiteWeightAPI.Commands.Users.SendFriendRequest;

public class SendFriendRequest : ICommand<FriendResponse>
{
	public required string SenderId { get; init; }
	public required string RecipientId { get; init; }
}