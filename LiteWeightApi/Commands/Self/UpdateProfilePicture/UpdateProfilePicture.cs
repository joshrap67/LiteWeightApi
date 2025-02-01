namespace LiteWeightAPI.Commands.Self.UpdateProfilePicture;

public class UpdateProfilePicture : ICommand<bool>
{
	public required string UserId { get; set; }
	public required byte[] ImageData { get; set; }
}