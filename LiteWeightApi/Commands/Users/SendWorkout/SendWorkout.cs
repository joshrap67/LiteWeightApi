namespace LiteWeightAPI.Commands.Users.SendWorkout;

public class SendWorkout : ICommand<string>
{
	public string SenderUserId { get; set; }
	public string RecipientUserId { get; set; }
	public string WorkoutId { get; set; }
}