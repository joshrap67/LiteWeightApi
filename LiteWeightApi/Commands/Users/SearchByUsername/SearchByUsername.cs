using LiteWeightAPI.Api.Users.Responses;

namespace LiteWeightAPI.Commands.Users.SearchByUsername;

public class SearchByUsername : ICommand<SearchUserResponse?>
{
	public required string Username { get; init; }
	public required string InitiatorId { get; init; }
}