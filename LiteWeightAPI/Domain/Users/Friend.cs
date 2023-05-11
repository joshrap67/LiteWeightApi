using Google.Cloud.Firestore;

namespace LiteWeightAPI.Domain.Users;

[FirestoreData]
public class Friend
{
	[FirestoreProperty("userId")] public string UserId { get; set; }
	[FirestoreProperty("username")] public string Username { get; set; }
	[FirestoreProperty("icon")] public string Icon { get; set; }
	[FirestoreProperty("confirmed")] public bool Confirmed { get; set; }
}