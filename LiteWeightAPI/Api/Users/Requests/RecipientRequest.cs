using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.Users.Requests;

public class RecipientRequest
{
	/// <summary>
	/// Username of the recipient.
	/// </summary>
	/// <example>barbell_bill</example>
	[Required]
	public string Username { get; set; }
}