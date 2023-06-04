using System.ComponentModel.DataAnnotations;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Api.Self.Requests;

public class SetUsernameRequest
{
	/// <summary>
	/// New username to set - must be unique.
	/// </summary>
	/// <example>waltuh</example>
	[Required]
	[MaxLength(Globals.MaxUsernameLength)]
	public string NewUsername { get; set; }
}