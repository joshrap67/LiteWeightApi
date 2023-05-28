namespace LiteWeightAPI.Commands.Self.SetAllReceivedWorkoutsSeen;

public class SetAllReceivedWorkoutsSeen : ICommand<bool>
{
	public string UserId { get; set; }
}