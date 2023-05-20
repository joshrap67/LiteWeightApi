using LiteWeightAPI.Errors.Attributes.Setup;

namespace LiteWeightAPI.Errors.Attributes;

public class InvalidRoutineAttribute : BaseErrorAttribute
{
    public InvalidRoutineAttribute() : base(ErrorTypes.InvalidRequest)
    {
    }
}