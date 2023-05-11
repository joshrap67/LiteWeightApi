using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.Users.Requests;

public class BlockUserRequest
{
	/// <summary>
	/// Username of the user to block
	/// </summary>
	/// <example>dee_bird</example>
	[Required]
	public string Username { get; set; }
}