using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.Users.Requests;

public class ReportUserRequest
{
	/// <summary>
	/// Description of the complaint.
	/// </summary>
	/// <example>Inappropriate username.</example>
	[Required]
	public string Description { get; set; }
}