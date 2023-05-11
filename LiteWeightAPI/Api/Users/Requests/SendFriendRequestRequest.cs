using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.Users.Requests;

public class SendFriendRequestRequest // todo rename lmao
{
	/// <summary>
	/// Username to send the friend request to. Username must belong to a valid LiteWeight user.
	/// </summary>
	/// <example>greg_egg</example>
	[Required]
	public string RecipientUsername { get; set; }
}