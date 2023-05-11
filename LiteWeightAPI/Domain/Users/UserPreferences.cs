using Google.Cloud.Firestore;

namespace LiteWeightAPI.Domain.Users;

[FirestoreData]
public class UserPreferences
{
	[FirestoreProperty("privateAccount")] public bool PrivateAccount { get; set; }
	[FirestoreProperty("updateDefaultWeightOnSave")] public bool UpdateDefaultWeightOnSave { get; set; }
	[FirestoreProperty("updateDefaultWeightOnRestart")] public bool UpdateDefaultWeightOnRestart { get; set; }
	[FirestoreProperty("metricUnits")] public bool MetricUnits { get; set; }
}