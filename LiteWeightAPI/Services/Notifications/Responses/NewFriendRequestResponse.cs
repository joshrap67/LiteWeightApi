namespace LiteWeightAPI.Services.Notifications.Responses;

public class NewFriendRequestResponse
{
	public string Username { get; set; }
	public string Icon { get; set; }
	public bool Seen { get; set; }
	public string RequestTimeStamp { get; set; }
}