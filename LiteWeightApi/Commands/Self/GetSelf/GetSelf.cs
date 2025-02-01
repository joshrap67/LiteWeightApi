using LiteWeightAPI.Api.Self.Responses;

namespace LiteWeightAPI.Commands.Self.GetSelf;

public class GetSelf : ICommand<UserResponse>
{
	public required string UserId { get; init; }
}