namespace LiteWeightAPI.Errors.Exceptions.BaseExceptions;

public class ResourceNotFoundException : Exception
{
    public string FormattedMessage { get; }

    public ResourceNotFoundException(string id) : base(GetFormattedMessage(id))
    {
        FormattedMessage = GetFormattedMessage(id);
    }

    private static string GetFormattedMessage(string id)
    {
        return $"Resource with id={id} not found";
    }
}