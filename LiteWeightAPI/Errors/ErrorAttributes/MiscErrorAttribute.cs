using LiteWeightAPI.Errors.ErrorAttributes.Setup;

namespace LiteWeightAPI.Errors.ErrorAttributes;

public class MiscErrorAttribute : BaseErrorAttribute
{
	public MiscErrorAttribute() : base(ErrorTypes.MiscError)
	{
	}
}