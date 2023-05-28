namespace LiteWeightAPI.Commands.Workouts.RenameWorkout;

public class RenameWorkout : ICommand<bool>
{
	public string UserId { get; set; }
	public string WorkoutId { get; set; }
	public string NewName { get; set; }
}