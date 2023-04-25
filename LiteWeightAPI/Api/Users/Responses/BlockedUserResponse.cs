namespace LiteWeightAPI.Api.Users.Responses;

public class BlockedUserResponse
{
	/// <summary>
	/// Username of the user.
	/// </summary>
	/// <example>coastal_trainer</example>
	public string Username { get; set; }

	/// <summary>
	/// File path of the user's icon
	/// </summary>
	/// <example>2a44b010-6578-48c1-a4d7-a8e07a450bee.jpg</example>
	public string Icon { get; set; }
}