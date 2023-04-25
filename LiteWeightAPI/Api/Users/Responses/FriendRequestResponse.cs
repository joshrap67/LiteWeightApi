namespace LiteWeightAPI.Api.Users.Responses;

public class FriendRequestResponse
{
	/// <summary>
	/// Username of the user.
	/// </summary>
	/// <example>arthur_v</example>
	public string Username { get; set; }

	/// <summary>
	/// File path of the user's icon
	/// </summary>
	/// <example>f5b17d02-0a8f-45b5-a2c9-3410fceb5cd3.jpg</example>
	public string Icon { get; set; }

	/// <summary>
	/// Is this friend request seen?
	/// </summary>
	public bool Seen { get; set; }

	/// <summary>
	/// Timestamp of when the request was sent (Zulu time).
	/// </summary>
	/// <example>2023-04-19T13:43:44.685341Z</example>
	public string RequestTimeStamp { get; set; }
}