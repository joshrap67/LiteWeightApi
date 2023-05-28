namespace LiteWeightAPI.Commands.SharedWorkouts.DeclineSharedWorkout;

public class DeclineSharedWorkout : ICommand<bool>
{
	public string UserId { get; set; }
	public string SharedWorkoutId { get; set; }
}