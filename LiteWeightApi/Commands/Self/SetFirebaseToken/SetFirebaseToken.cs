namespace LiteWeightAPI.Commands.Self.SetFirebaseToken;

public class SetFirebaseToken : ICommand<bool>
{
	public string UserId { get; set; }
	public string Token { get; set; }
}