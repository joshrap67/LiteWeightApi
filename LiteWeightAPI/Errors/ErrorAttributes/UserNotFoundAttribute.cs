using LiteWeightAPI.Errors.ErrorAttributes.Setup;

namespace LiteWeightAPI.Errors.ErrorAttributes;

public class UserNotFoundAttribute : BaseErrorAttribute
{
	public UserNotFoundAttribute() : base(ErrorTypes.UserNotFound)
	{
	}
}