namespace LiteWeightAPI.Commands.Self.SetAllReceivedWorkoutsSeen;

public class SetAllReceivedWorkoutsSeen : ICommand<bool>
{
	public required string UserId { get; set; }
}