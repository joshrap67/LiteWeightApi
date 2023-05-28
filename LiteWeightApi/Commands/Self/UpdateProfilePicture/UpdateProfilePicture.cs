namespace LiteWeightAPI.Commands.Self.UpdateProfilePicture;

public class UpdateProfilePicture : ICommand<bool>
{
	public string UserId { get; set; }
	public byte[] ImageData { get; set; }
}