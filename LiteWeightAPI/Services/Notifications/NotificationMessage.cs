using System.Text.Json.Serialization;

namespace LiteWeightAPI.Services.Notifications;

public class NotificationMessage
{
	[JsonPropertyName("GCM")] public InnerMessage InnerMsg { get; set; }

	public class InnerMessage
	{
		[JsonPropertyName("default")] public string DefaultKey { get; set; } = "default message";
		[JsonPropertyName("metadata")] public string Metadata { get; set; }
	}
}