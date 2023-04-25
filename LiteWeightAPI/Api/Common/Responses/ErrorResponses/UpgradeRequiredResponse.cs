namespace LiteWeightAPI.Api.Common.Responses.ErrorResponses;

public class UpgradeRequiredResponse
{
	/// <summary>
	/// Message describing the error.
	/// </summary>
	/// <example>LiteWeight android app needs needs to be updated.</example>
	public string Message { get; set; }

	/// <summary>
	/// Minimum version required to use this API.
	/// </summary>
	/// <example>12</example>
	public int MinimumVersionRequired { get; set; }
}