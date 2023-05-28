namespace LiteWeightAPI.Commands.Users.ShareWorkout;

public class ShareWorkout : ICommand<string>
{
	public string SenderUserId { get; set; }
	public string RecipientUserId { get; set; }
	public string WorkoutId { get; set; }
}