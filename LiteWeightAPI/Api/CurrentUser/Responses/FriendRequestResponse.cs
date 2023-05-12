﻿namespace LiteWeightAPI.Api.CurrentUser.Responses;

public class FriendRequestResponse
{
	/// <summary>
	/// Unique identifier of the user.
	/// </summary>
	/// <example>dcceafca-7055-4c0d-81f8-0e9ef16c7bdc</example>
	public string UserId { get; set; }

	/// <summary>
	/// Username of the user.
	/// </summary>
	/// <example>arthur_v</example>
	public string Username { get; set; }

	/// <summary>
	/// Url of the user's icon
	/// </summary>
	/// <example>https://storage.googleapis.com/liteweight-profile-pictures/f5b17d02-0a8f-45b5-a2c9-3410fceb5cd3.jpg</example>
	public string UserIcon { get; set; }

	/// <summary>
	/// Is this friend request seen?
	/// </summary>
	public bool Seen { get; set; }

	/// <summary>
	/// Timestamp of when the request was sent (Zulu time).
	/// </summary>
	/// <example>2023-04-19T13:43:44.685341Z</example>
	public string SentUtc { get; set; }
}