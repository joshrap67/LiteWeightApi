using LiteWeightAPI.Errors.ErrorAttributes.Setup;

namespace LiteWeightAPI.Errors.ErrorAttributes;

public class DuplicateFoundAttribute : BaseErrorAttribute
{
	public DuplicateFoundAttribute() : base(ErrorTypes.DuplicateFound)
	{
	}
}