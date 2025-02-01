namespace LiteWeightAPI.Commands.Workouts.DeleteWorkoutAndSetCurrent;

public class DeleteWorkoutAndSetCurrent : ICommand<bool>
{
	public required string UserId { get; set; }
	public required string WorkoutToDeleteId { get; set; }
	public required string CurrentWorkoutId { get; set; }
}