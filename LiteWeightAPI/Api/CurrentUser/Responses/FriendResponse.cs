namespace LiteWeightAPI.Api.CurrentUser.Responses;

public class FriendResponse
{
	/// <summary>
	/// Unique identifier of the friend.
	/// </summary>
	/// <example>53c68af6-9400-438b-aece-344d4d2024c6</example>
	public string UserId { get; set; }

	/// <summary>
	/// Username of the friend.
	/// </summary>
	/// <example>greg_egg</example>
	public string Username { get; set; }

	/// <summary>
	/// Url of the friend's icon
	/// </summary>
	/// <example>https://storage.googleapis.com/liteweight-profile-pictures/66fcc4c3-700e-41e3-b0e5-9f121eb97fa9.jpg</example>
	public string UserIcon { get; set; }

	/// <summary>
	/// Is the friend confirmed? If yes they are a friend, else they are a pending friend (pending until this user accepts friend request).
	/// </summary>
	public bool Confirmed { get; set; }
}