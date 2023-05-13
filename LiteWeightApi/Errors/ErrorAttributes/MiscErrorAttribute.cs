using LiteWeightApi.Errors.ErrorAttributes.Setup;

namespace LiteWeightApi.Errors.ErrorAttributes;

public class MiscErrorAttribute : BaseErrorAttribute
{
	public MiscErrorAttribute() : base(ErrorTypes.MiscError)
	{
	}
}