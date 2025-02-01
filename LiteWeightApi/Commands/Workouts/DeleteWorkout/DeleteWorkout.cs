namespace LiteWeightAPI.Commands.Workouts.DeleteWorkout;

public class DeleteWorkout : ICommand<bool>
{
	public required string UserId { get; set; }
	public required string WorkoutId { get; set; }
}