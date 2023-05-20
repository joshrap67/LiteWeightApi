namespace LiteWeightAPI.Errors.Attributes.Setup;

[AttributeUsage(AttributeTargets.Method)]
public abstract class BaseErrorAttribute : Attribute
{
    public string ErrorType { get; }

    protected BaseErrorAttribute(string errorType)
    {
        ErrorType = errorType;
    }
}