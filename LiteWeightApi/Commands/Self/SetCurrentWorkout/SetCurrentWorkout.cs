namespace LiteWeightAPI.Commands.Self.SetCurrentWorkout;

public class SetCurrentWorkout : ICommand<bool>
{
	public string UserId { get; set; }
	public string CurrentWorkoutId { get; set; }
}