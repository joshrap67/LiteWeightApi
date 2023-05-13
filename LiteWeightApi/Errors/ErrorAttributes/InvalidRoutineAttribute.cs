using LiteWeightApi.Errors.ErrorAttributes.Setup;

namespace LiteWeightApi.Errors.ErrorAttributes;

public class InvalidRoutineAttribute : BaseErrorAttribute
{
    public InvalidRoutineAttribute() : base(ErrorTypes.InvalidRequest)
    {
    }
}