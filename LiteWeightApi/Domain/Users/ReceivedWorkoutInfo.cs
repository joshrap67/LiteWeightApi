using Google.Cloud.Firestore;
using LiteWeightAPI.Domain.Converters;
using NodaTime;

namespace LiteWeightAPI.Domain.Users;

[FirestoreData]
public class ReceivedWorkoutInfo
{
	[FirestoreProperty("receivedWorkoutId")]
	public string ReceivedWorkoutId { get; set; }

	[FirestoreProperty("workoutName")]
	public string WorkoutName { get; set; }

	[FirestoreProperty("receivedUtc", ConverterType = typeof(InstantConverter))]
	public Instant ReceivedUtc { get; set; }

	[FirestoreProperty("seen")]
	public bool Seen { get; set; }

	[FirestoreProperty("senderId")]
	public string SenderId { get; set; }

	[FirestoreProperty("senderUsername")]
	public string SenderUsername { get; set; }

	[FirestoreProperty("senderProfilePicture")]
	public string SenderProfilePicture { get; set; }

	[FirestoreProperty("totalDays")]
	public int TotalDays { get; set; }

	[FirestoreProperty("mostFrequentFocus")]
	public string MostFrequentFocus { get; set; }
}