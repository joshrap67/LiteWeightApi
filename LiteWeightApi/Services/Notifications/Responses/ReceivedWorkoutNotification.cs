namespace LiteWeightApi.Services.Notifications.Responses;

public class ReceivedWorkoutNotification
{
	public string SharedWorkoutId { get; set; }
	public string WorkoutName { get; set; }
	public string SentTimestamp { get; set; }
	public bool Seen { get; set; }
	public string SenderId { get; set; }
	public string SenderUsername { get; set; }
	public string SenderIcon { get; set; }
	public int TotalDays { get; set; }
	public string MostFrequentFocus { get; set; }
}