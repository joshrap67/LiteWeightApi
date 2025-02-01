namespace LiteWeightAPI.Commands.Workouts.ResetStatistics;

public class ResetStatistics : ICommand<bool>
{
	public required string UserId { get; set; }
	public required string WorkoutId { get; set; }
}