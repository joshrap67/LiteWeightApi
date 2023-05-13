using LiteWeightApi.Errors.ErrorAttributes.Setup;

namespace LiteWeightApi.Errors.ErrorAttributes;

public class AlreadyExistsAttribute : BaseErrorAttribute
{
	public AlreadyExistsAttribute() : base(ErrorTypes.AlreadyExists)
	{
	}
}