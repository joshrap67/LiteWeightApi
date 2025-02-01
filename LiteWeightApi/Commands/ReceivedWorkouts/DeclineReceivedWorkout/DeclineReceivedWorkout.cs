namespace LiteWeightAPI.Commands.ReceivedWorkouts.DeclineReceivedWorkout;

public class DeclineReceivedWorkout : ICommand<bool>
{
	public required string UserId { get; set; }
	public required string ReceivedWorkoutId { get; set; }
}