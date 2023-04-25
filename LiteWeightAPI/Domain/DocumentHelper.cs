using System.Text.Json;
using Amazon.DynamoDBv2.DocumentModel;

namespace LiteWeightAPI.Domain;

public static class DocumentHelper
{
	private static readonly JsonSerializerOptions SerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = true
	};

	public static Document Serialize<T>(T obj)
	{
		var json = JsonSerializer.Serialize(obj, SerializerOptions);
		var document = Document.FromJson(json);
		return document;
	}

	public static T Deserialize<T>(Document document)
	{
		var json = document.ToJson();
		var obj = JsonSerializer.Deserialize<T>(json, SerializerOptions);
		return obj;
	}
}