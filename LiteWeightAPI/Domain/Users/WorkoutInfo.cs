using Google.Cloud.Firestore;
using LiteWeightAPI.Domain.Converters;
using NodaTime;

namespace LiteWeightAPI.Domain.Users;

[FirestoreData]
public class WorkoutInfo
{
	[FirestoreProperty("workoutId")] public string WorkoutId { get; set; }
	[FirestoreProperty("workoutName")] public string WorkoutName { get; set; }

	[FirestoreProperty("lastSetAsCurrentUtc", ConverterType = typeof(InstantConverter))]
	public Instant LastSetAsCurrentUtc { get; set; }

	[FirestoreProperty("timesCompleted")] public int TimesCompleted { get; set; }

	[FirestoreProperty("averageExercisesCompleted")]
	public double AverageExercisesCompleted { get; set; }

	[FirestoreProperty("totalExercisesSum")]
	public int TotalExercisesSum { get; set; }
}