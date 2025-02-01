namespace LiteWeightAPI.Commands.Workouts.UpdateWorkoutProgress;

public class UpdateWorkoutProgress : ICommand<bool>
{
	public required string UserId { get; set; }
	public required string WorkoutId { get; set; }
	public int CurrentWeek { get; set; }
	public int CurrentDay { get; set; }
	public required SetRoutine Routine { get; set; }
}