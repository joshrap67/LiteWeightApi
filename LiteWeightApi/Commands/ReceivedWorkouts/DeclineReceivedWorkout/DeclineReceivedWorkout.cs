namespace LiteWeightAPI.Commands.ReceivedWorkouts.DeclineReceivedWorkout;

public class DeclineReceivedWorkout : ICommand<bool>
{
	public string UserId { get; set; }
	public string ReceivedWorkoutId { get; set; }
}