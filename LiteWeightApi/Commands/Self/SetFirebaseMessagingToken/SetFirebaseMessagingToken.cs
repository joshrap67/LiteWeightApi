namespace LiteWeightAPI.Commands.Self.SetFirebaseMessagingToken;

public class SetFirebaseMessagingToken : ICommand<bool>
{
	public required string UserId { get; set; }
	public required string Token { get; set; }
}