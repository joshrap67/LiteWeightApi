using LiteWeightAPI.Api.Users.Responses;

namespace LiteWeightAPI.Commands.Users.SearchByUsername;

public class SearchByUsername : ICommand<SearchUserResponse>
{
	public string Username { get; init; }
	public string InitiatorId { get; init; }
}