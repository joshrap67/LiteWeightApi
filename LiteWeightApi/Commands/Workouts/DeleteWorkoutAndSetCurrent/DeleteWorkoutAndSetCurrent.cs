namespace LiteWeightAPI.Commands.Workouts.DeleteWorkoutAndSetCurrent;

public class DeleteWorkoutAndSetCurrent : ICommand<bool>
{
	public string UserId { get; set; }
	public string WorkoutToDeleteId { get; set; }
	public string CurrentWorkoutId { get; set; }
}