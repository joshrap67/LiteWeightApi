namespace LiteWeightAPI.Domain.Users;

public class SharedWorkoutMeta
{
	public string WorkoutId { get; set; }
	public string WorkoutName { get; set; }
	public string DateSent { get; set; }
	public bool Seen { get; set; }
	public string Sender { get; set; }
	public int TotalDays { get; set; }
	public string MostFrequentFocus { get; set; }
	public string Icon { get; set; }
}