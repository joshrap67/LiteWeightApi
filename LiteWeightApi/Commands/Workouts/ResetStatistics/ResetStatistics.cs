namespace LiteWeightAPI.Commands.Workouts.ResetStatistics;

public class ResetStatistics : ICommand<bool>
{
	public string UserId { get; set; }
	public string WorkoutId { get; set; }
}