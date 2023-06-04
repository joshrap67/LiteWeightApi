namespace LiteWeightAPI.Commands.Self.SetUsername;

public class SetUsername : ICommand<bool>
{
	public string UserId { get; set; }
	public string NewUsername { get; set; }
}