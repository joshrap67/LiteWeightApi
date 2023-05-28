using LiteWeightAPI.Api.Self.Responses;

namespace LiteWeightAPI.Commands.Self.CreateSelf;

public class CreateSelf : ICommand<UserResponse>
{
	public string UserId { get; set; }
	
	public string UserEmail { get; set; }
	
	public string Username { get; set; }

	public byte[] ProfilePictureData { get; set; }

	public bool MetricUnits { get; set; }
}