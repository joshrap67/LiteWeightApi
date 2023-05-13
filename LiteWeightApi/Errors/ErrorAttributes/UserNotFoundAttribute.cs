using LiteWeightApi.Errors.ErrorAttributes.Setup;

namespace LiteWeightApi.Errors.ErrorAttributes;

public class UserNotFoundAttribute : BaseErrorAttribute
{
	public UserNotFoundAttribute() : base(ErrorTypes.UserNotFound)
	{
	}
}