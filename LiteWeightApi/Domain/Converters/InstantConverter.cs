using Google.Cloud.Firestore;
using LiteWeightApi.Utils;
using NodaTime;

namespace LiteWeightApi.Domain.Converters;

public class InstantConverter : IFirestoreConverter<Instant>
{
	public object ToFirestore(Instant value)
	{
		return value.ToString();
	}

	public Instant FromFirestore(object value)
	{
		return value switch
		{
			string timestamp => ParsingService.ParseStringToInstant(timestamp),
			null => throw new ArgumentNullException(nameof(value)),
			_ => throw new ArgumentException($"Unexpected data: {value.GetType()}")
		};
	}
}