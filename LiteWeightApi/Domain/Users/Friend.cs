using Google.Cloud.Firestore;

namespace LiteWeightApi.Domain.Users;

[FirestoreData]
public class Friend
{
	[FirestoreProperty("userId")] public string UserId { get; set; }
	[FirestoreProperty("username")] public string Username { get; set; }
	[FirestoreProperty("userIcon")] public string UserIcon { get; set; }
	[FirestoreProperty("confirmed")] public bool Confirmed { get; set; }
}