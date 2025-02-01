using LiteWeightAPI.Api.Self.Responses;

namespace LiteWeightAPI.Commands.Self.CreateSelf;

public class CreateSelf : ICommand<UserResponse>
{
	public required string UserId { get; set; }
	
	public required string UserEmail { get; set; }
	
	public required string Username { get; set; }

	public byte[]? ProfilePictureData { get; set; }

	public bool MetricUnits { get; set; }
}