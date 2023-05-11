using Google.Cloud.Firestore;
using NodaTime;

namespace LiteWeightAPI.Domain.Users;

[FirestoreData]
public class FriendRequest
{
	[FirestoreProperty("userId")] public string UserId { get; set; }
	[FirestoreProperty("username")] public string Username { get; set; }
	[FirestoreProperty("icon")] public string Icon { get; set; }
	[FirestoreProperty("seen")] public bool Seen { get; set; }
	[FirestoreProperty("requestTimestamp")] public Instant RequestTimestamp { get; set; }
}