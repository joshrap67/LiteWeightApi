namespace LiteWeightAPI.Commands.Self.SetReceivedWorkoutSeen;

public class SetReceivedWorkoutSeen : ICommand<bool>
{
	public string UserId { get; set; }
	public string SharedWorkoutId { get; set; }
}