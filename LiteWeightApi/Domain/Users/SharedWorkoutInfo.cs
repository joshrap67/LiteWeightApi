using Google.Cloud.Firestore;
using LiteWeightApi.Domain.Converters;
using NodaTime;

namespace LiteWeightApi.Domain.Users;

[FirestoreData]
public class SharedWorkoutInfo
{
	[FirestoreProperty("sharedWorkoutId")] public string SharedWorkoutId { get; set; }
	[FirestoreProperty("workoutName")] public string WorkoutName { get; set; }

	[FirestoreProperty("sharedUtc", ConverterType = typeof(InstantConverter))]
	public Instant SharedUtc { get; set; }

	[FirestoreProperty("seen")] public bool Seen { get; set; }
	[FirestoreProperty("senderId")] public string SenderId { get; set; }
	[FirestoreProperty("senderUsername")] public string SenderUsername { get; set; }
	[FirestoreProperty("senderIcon")] public string SenderIcon { get; set; }
	[FirestoreProperty("totalDays")] public int TotalDays { get; set; }

	[FirestoreProperty("mostFrequentFocus")]
	public string MostFrequentFocus { get; set; }
}