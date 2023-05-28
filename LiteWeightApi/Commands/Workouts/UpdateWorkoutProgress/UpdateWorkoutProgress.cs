namespace LiteWeightAPI.Commands.Workouts.UpdateWorkoutProgress;

public class UpdateWorkoutProgress : ICommand<bool>
{
	public string UserId { get; set; }
	public string WorkoutId { get; set; }
	public int CurrentWeek { get; set; }
	public int CurrentDay { get; set; }
	public SetRoutine Routine { get; set; }
}