namespace LiteWeightAPI.Commands.Workouts.DeleteWorkout;

public class DeleteWorkout : ICommand<bool>
{
	public string UserId { get; set; }
	public string WorkoutId { get; set; }
}