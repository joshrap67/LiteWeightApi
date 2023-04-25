using LiteWeightAPI.Errors.ErrorAttributes.Setup;

namespace LiteWeightAPI.Errors.ErrorAttributes;

public class InvalidRequestAttribute : BaseErrorAttribute
{
    public InvalidRequestAttribute() : base(ErrorTypes.InvalidRequest)
    {
    }
}