namespace LiteWeightAPI.Commands.Self.SetSettings;

public class SetSettings : ICommand<bool>
{
	public string UserId { get; set; }

	public bool PrivateAccount { get; set; }

	public bool UpdateDefaultWeightOnSave { get; set; }

	public bool UpdateDefaultWeightOnRestart { get; set; }

	public bool MetricUnits { get; set; }
}