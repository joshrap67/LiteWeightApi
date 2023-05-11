using Google.Cloud.Firestore;
using NodaTime;

namespace LiteWeightAPI.Domain.Users;

[FirestoreData]
public class SharedWorkoutInfo
{
	[FirestoreProperty("sharedWorkoutId")] public string SharedWorkoutId { get; set; }
	[FirestoreProperty("workoutName")] public string WorkoutName { get; set; }

	[FirestoreProperty("sentTimestamp")] public Instant SentTimestamp { get; set; }

	// todo nodatime serialization
	[FirestoreProperty("seen")] public bool Seen { get; set; }
	[FirestoreProperty("senderId")] public string SenderId { get; set; }
	[FirestoreProperty("senderUsername")] public string SenderUsername { get; set; }
	[FirestoreProperty("senderIcon")] public string SenderIcon { get; set; }
	[FirestoreProperty("totalDays")] public int TotalDays { get; set; }

	[FirestoreProperty("mostFrequentFocus")]
	public string MostFrequentFocus { get; set; }
}