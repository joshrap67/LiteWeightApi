namespace LiteWeightApi.Errors.Exceptions.BaseExceptions;

public class ResourceNotFoundException : Exception
{
	private const string DefaultMessage = "Resource not found";

	public string FormattedMessage { get; } = DefaultMessage;

	public ResourceNotFoundException() : base(DefaultMessage)
	{
	}

	public ResourceNotFoundException(string resourceName) : base(GetFormattedMessage(resourceName))
	{
		FormattedMessage = GetFormattedMessage(resourceName);
	}

	private static string GetFormattedMessage(string resourceName)
	{
		return $"{resourceName} not found";
	}
}