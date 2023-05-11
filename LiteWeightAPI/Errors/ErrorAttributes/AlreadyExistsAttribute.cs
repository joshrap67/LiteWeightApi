using LiteWeightAPI.Errors.ErrorAttributes.Setup;

namespace LiteWeightAPI.Errors.ErrorAttributes;

public class AlreadyExistsAttribute : BaseErrorAttribute
{
	public AlreadyExistsAttribute() : base(ErrorTypes.AlreadyExists)
	{
	}
}