namespace LiteWeightAPI.Api.Users.Responses;

public class ComplaintResponse
{
	/// <summary>
	/// Id of the complaint.
	/// </summary>
	/// <example>29062e23-fa61-4977-af93-aa4fdb177247</example>
	public string Id { get; set; }

	/// <summary>
	/// User id of the user who made the report.
	/// </summary>
	/// <example>3f96d8c2-127c-4605-8272-003630d8c1a1</example>
	public string ClaimantUserId { get; set; }

	/// <summary>
	/// User id of the user who was reported.
	/// </summary>
	/// <example>b36291c6-19ee-4bd0-b1f2-1d092a2e831e</example>
	public string ReportedUserId { get; set; }

	/// <summary>
	/// Username of the user who was reported. Note this is the username at the time of being reported.
	/// </summary>
	/// <example>pepe_silvia</example>
	public string ReportedUsername { get; set; }

	/// <summary>
	/// Timestamp of when the report was created (Zulu).
	/// </summary>
	/// <example>2023-05-20T08:43:44.685341Z</example>
	public string ReportedUtc { get; set; }
}