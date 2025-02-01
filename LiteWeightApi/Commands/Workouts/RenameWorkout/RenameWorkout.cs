namespace LiteWeightAPI.Commands.Workouts.RenameWorkout;

public class RenameWorkout : ICommand<bool>
{
	public required string UserId { get; set; }
	public required string WorkoutId { get; set; }
	public required string NewName { get; set; }
}