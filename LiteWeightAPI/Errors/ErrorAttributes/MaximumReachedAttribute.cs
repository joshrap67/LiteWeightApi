using LiteWeightAPI.Errors.ErrorAttributes.Setup;

namespace LiteWeightAPI.Errors.ErrorAttributes;

public class MaximumReachedAttribute : BaseErrorAttribute
{
	public MaximumReachedAttribute() : base(ErrorTypes.MaximumReached)
	{
	}
}