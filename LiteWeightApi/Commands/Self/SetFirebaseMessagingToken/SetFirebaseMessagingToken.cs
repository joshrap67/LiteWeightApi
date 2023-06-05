namespace LiteWeightAPI.Commands.Self.SetFirebaseMessagingToken;

public class SetFirebaseMessagingToken : ICommand<bool>
{
	public string UserId { get; set; }
	public string Token { get; set; }
}