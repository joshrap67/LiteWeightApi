namespace LiteWeightAPI.Commands.Self.SetReceivedWorkoutSeen;

public class SetReceivedWorkoutSeen : ICommand<bool>
{
	public required string UserId { get; set; }
	public required string ReceivedWorkoutId { get; set; }
}