namespace LiteWeightAPI.Domain.Users;

public class FriendRequest
{
	public string Username { get; set; }
	public string Icon { get; set; }
	public bool Seen { get; set; }
	public string RequestTimeStamp { get; set; }
}