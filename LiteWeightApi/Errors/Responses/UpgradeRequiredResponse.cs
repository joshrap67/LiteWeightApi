namespace LiteWeightAPI.Errors.Responses;

public class UpgradeRequiredResponse
{
	// todo this is now unnecessary with the frontend reading directly from firebase for first load
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