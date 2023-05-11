using LiteWeightAPI.Errors.ErrorAttributes.Setup;

namespace LiteWeightAPI.Errors.ErrorAttributes;

public class MaxLimitAttribute : BaseErrorAttribute
{
	public MaxLimitAttribute() : base(ErrorTypes.MaxLimit)
	{
	}
}