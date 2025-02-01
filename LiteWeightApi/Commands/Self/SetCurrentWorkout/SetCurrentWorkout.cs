namespace LiteWeightAPI.Commands.Self.SetCurrentWorkout;

public class SetCurrentWorkout : ICommand<bool>
{
	public required string UserId { get; set; }
	public required string CurrentWorkoutId { get; set; }
}