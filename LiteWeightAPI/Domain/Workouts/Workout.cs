using Google.Cloud.Firestore;
using NodaTime;

namespace LiteWeightAPI.Domain.Workouts;

[FirestoreData]
public class Workout
{
	[FirestoreDocumentId] public string Id { get; set; }
	[FirestoreProperty("name")] public string Name { get; set; }
	[FirestoreProperty("creationTimestamp")] public Instant CreationTimestamp { get; set; }
	[FirestoreProperty("lastModified")] public Instant LastModified { get; set; }
	[FirestoreProperty("creatorId")] public string CreatorId { get; set; }
	[FirestoreProperty("routine")] public Routine Routine { get; set; }
	[FirestoreProperty("currentDay")] public int CurrentDay { get; set; }
	[FirestoreProperty("currentWeek")] public int CurrentWeek { get; set; }
}