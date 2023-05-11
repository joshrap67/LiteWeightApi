using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.CurrentUser.Requests;

public class CreateUserRequest
{
	/// <summary>
	/// Username of the new user - must be unique.
	/// </summary>
	/// <example>randy_bo_bandy</example>
	[Required]
	public string Username { get; set; }

	/// <summary>
	/// Should the created user have metric units enabled?
	/// </summary>
	public bool MetricUnits { get; set; }
}