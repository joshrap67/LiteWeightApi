namespace LiteWeightAPI.Api.CurrentUser.Responses;

public class UserPreferencesResponse
{
	/// <summary>
	/// If true, the user will not receive any friend requests or workouts from users who are not friends with this user.
	/// </summary>
	public bool PrivateAccount { get; set; }

	/// <summary>
	/// If true, the default value of exercises will be updated when saving a workout if the completed exercise's weight is greater than the default weight.
	/// </summary>
	/// <example>true</example>
	public bool UpdateDefaultWeightOnSave { get; set; }

	/// <summary>
	/// If true, the default value of exercises will be updated when restarting a workout if the completed exercise's weight is greater than the default weight.
	/// </summary>
	/// <example>true</example>
	public bool UpdateDefaultWeightOnRestart { get; set; }

	/// <summary>
	/// If true, display weights as metric (only used on the UI - values returned from this API are always in imperial units).
	/// </summary>
	public bool MetricUnits { get; set; }
}