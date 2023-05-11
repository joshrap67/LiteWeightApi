using LiteWeightAPI.Errors.ErrorAttributes.Setup;

namespace LiteWeightAPI.Errors.ErrorAttributes;

public class InvalidRoutineAttribute : BaseErrorAttribute
{
    public InvalidRoutineAttribute() : base(ErrorTypes.InvalidRequest)
    {
    }
}