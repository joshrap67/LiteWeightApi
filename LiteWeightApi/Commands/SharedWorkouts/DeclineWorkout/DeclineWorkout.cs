namespace LiteWeightAPI.Commands.SharedWorkouts.DeclineWorkout;

public class DeclineWorkout : ICommand<bool>
{
	public string UserId { get; set; }
	public string SharedWorkoutId { get; set; }
}