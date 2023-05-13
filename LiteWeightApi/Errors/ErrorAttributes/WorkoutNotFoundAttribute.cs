using LiteWeightApi.Errors.ErrorAttributes.Setup;

namespace LiteWeightApi.Errors.ErrorAttributes;

public class WorkoutNotFoundAttribute : BaseErrorAttribute
{
	public WorkoutNotFoundAttribute() : base(ErrorTypes.WorkoutNotFound)
	{
	}
}