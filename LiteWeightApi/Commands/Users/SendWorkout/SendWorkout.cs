namespace LiteWeightAPI.Commands.Users.SendWorkout;

public class SendWorkout : ICommand<string>
{
	public required string SenderUserId { get; set; }
	public required string RecipientUserId { get; set; }
	public required string WorkoutId { get; set; }
}