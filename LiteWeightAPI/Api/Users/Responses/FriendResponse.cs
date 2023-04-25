namespace LiteWeightAPI.Api.Users.Responses;

public class FriendResponse
{
	/// <summary>
	/// Username of the friend.
	/// </summary>
	/// <example>dennis_r</example>
	public string Username { get; set; }

	/// <summary>
	/// File path of the user's icon
	/// </summary>
	/// <example>66fcc4c3-700e-41e3-b0e5-9f121eb97fa9.jpg</example>
	public string Icon { get; set; }

	/// <summary>
	/// Is the user confirmed? If yes they are a friend, else they are a pending friend (pending until other user accepts friend request).
	/// </summary>
	public bool Confirmed { get; set; }
}