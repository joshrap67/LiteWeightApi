using LiteWeightApi.Errors.ErrorAttributes.Setup;

namespace LiteWeightApi.Errors.ErrorAttributes;

public class MaxLimitAttribute : BaseErrorAttribute
{
	public MaxLimitAttribute() : base(ErrorTypes.MaxLimit)
	{
	}
}