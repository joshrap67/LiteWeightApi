using LiteWeightAPI.Api.Self.Responses;

namespace LiteWeightAPI.Commands.Self.GetSelf;

public class GetSelf : ICommand<UserResponse>
{
	public string UserId { get; set; }
}