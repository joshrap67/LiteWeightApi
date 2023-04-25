namespace LiteWeightAPI.Domain.Users;

public class User
{
	public string Username { get; set; }
	public string Icon { get; set; }
	public string PushEndpointArn { get; set; }
	public string PremiumToken { get; set; }
	public string CurrentWorkout { get; set; }
	public int WorkoutsSent { get; set; }
	public UserPreferences UserPreferences { get; set; }
	public List<Blocked> Blocked { get; set; } = new();
	public List<WorkoutMeta> Workouts { get; set; } = new();
	public List<OwnedExercise> Exercises { get; set; } = new();
	public List<Friend> Friends { get; set; } = new();
	public List<FriendRequest> FriendRequests { get; set; } = new();
	public List<SharedWorkoutMeta> ReceivedWorkouts { get; set; } = new();
}