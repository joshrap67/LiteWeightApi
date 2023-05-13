using Google.Cloud.Firestore;

namespace LiteWeightApi.Domain.Users;

[FirestoreData]
public class User
{
	[FirestoreDocumentId] public string Id { get; set; }
	[FirestoreProperty("username")] public string Username { get; set; }
	[FirestoreProperty("email")] public string Email { get; set; }
	[FirestoreProperty("icon")] public string Icon { get; set; }
	[FirestoreProperty("firebaseMessagingToken")] public string FirebaseMessagingToken { get; set; }
	[FirestoreProperty("premiumToken")] public string PremiumToken { get; set; }
	[FirestoreProperty("currentWorkoutId")] public string CurrentWorkoutId { get; set; }
	[FirestoreProperty("workoutsSent")] public int WorkoutsSent { get; set; }
	[FirestoreProperty("userPreferences")] public UserPreferences UserPreferences { get; set; }
	[FirestoreProperty("workouts")] public List<WorkoutInfo> Workouts { get; set; } = new();
	[FirestoreProperty("exercises")] public List<OwnedExercise> Exercises { get; set; } = new();
	[FirestoreProperty("friends")] public List<Friend> Friends { get; set; } = new();
	[FirestoreProperty("friendRequests")] public List<FriendRequest> FriendRequests { get; set; } = new();
	[FirestoreProperty("receivedWorkouts")] public List<SharedWorkoutInfo> ReceivedWorkouts { get; set; } = new();
}