using Google.Cloud.Firestore;
using LiteWeightAPI.Domain.Converters;
using NodaTime;

namespace LiteWeightAPI.Domain.Workouts;

[FirestoreData]
public class Workout
{
	[FirestoreDocumentId]
	public string Id { get; set; }

	[FirestoreProperty("name")]
	public string Name { get; set; }

	[FirestoreProperty("creationUtc", ConverterType = typeof(InstantConverter))]
	public Instant CreationUtc { get; set; }

	[FirestoreProperty("creatorId")]
	public string CreatorId { get; set; }

	[FirestoreProperty("routine")]
	public Routine Routine { get; set; }

	[FirestoreProperty("currentDay")]
	public int CurrentDay { get; set; }

	[FirestoreProperty("currentWeek")]
	public int CurrentWeek { get; set; } // todo i think this should move to workout info...
}