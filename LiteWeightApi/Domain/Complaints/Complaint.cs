using Google.Cloud.Firestore;
using LiteWeightAPI.Domain.Converters;
using NodaTime;

namespace LiteWeightAPI.Domain.Complaints;

[FirestoreData]
public class Complaint
{
	[FirestoreDocumentId]
	public string Id { get; set; } = Guid.NewGuid().ToString();

	[FirestoreProperty("claimantUserId")]
	public string ClaimantUserId { get; set; }

	[FirestoreProperty("reportedUserId")]
	public string ReportedUserId { get; set; }

	[FirestoreProperty("reportedUsername")]
	public string ReportedUsername { get; set; }

	[FirestoreProperty("description")]
	public string Description { get; set; }

	// in the future could add profile picture if wanting to persist a potentially offensive profile picture

	[FirestoreProperty("reportedUtc", ConverterType = typeof(InstantConverter))]
	public Instant ReportedUtc { get; set; }
}