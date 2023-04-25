namespace LiteWeightAPI.Domain.Users;

public class UserPreferences
{
	public bool PrivateAccount { get; set; }
	public bool UpdateDefaultWeightOnSave { get; set; }
	public bool UpdateDefaultWeightOnRestart { get; set; }
	public bool MetricUnits { get; set; }
}