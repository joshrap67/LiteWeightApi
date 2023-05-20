using Google.Cloud.Firestore;
using LiteWeightAPI.Domain.Converters;
using NodaTime;

namespace LiteWeightAPI.Domain.Users;

[FirestoreData]
public class FriendRequest
{
	[FirestoreProperty("userId")]
	public string UserId { get; set; }

	[FirestoreProperty("username")]
	public string Username { get; set; }

	[FirestoreProperty("profilePicture")]
	public string ProfilePicture { get; set; }

	[FirestoreProperty("seen")]
	public bool Seen { get; set; }

	[FirestoreProperty("sentUtc", ConverterType = typeof(InstantConverter))]
	public Instant SentUtc { get; set; }
}