using LiteWeightApi.Errors.ErrorAttributes.Setup;

namespace LiteWeightApi.Errors.ErrorAttributes;

public class InvalidRequestAttribute : BaseErrorAttribute
{
    public InvalidRequestAttribute() : base(ErrorTypes.InvalidRequest)
    {
    }
}