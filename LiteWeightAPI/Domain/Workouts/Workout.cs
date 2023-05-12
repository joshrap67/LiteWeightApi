using Google.Cloud.Firestore;
using LiteWeightAPI.Domain.Converters;
using NodaTime;

namespace LiteWeightAPI.Domain.Workouts;

[FirestoreData]
public class Workout
{
	[FirestoreDocumentId] public string Id { get; set; }
	[FirestoreProperty("name")] public string Name { get; set; }

	[FirestoreProperty("creationUtc", ConverterType = typeof(InstantConverter))]
	public Instant CreationUtc { get; set; } // todo see if necessary since firestore apparently has this built in

	[FirestoreProperty("creatorId")] public string CreatorId { get; set; }
	[FirestoreProperty("routine")] public Routine Routine { get; set; }
	[FirestoreProperty("currentDay")] public int CurrentDay { get; set; }
	[FirestoreProperty("currentWeek")] public int CurrentWeek { get; set; }
}