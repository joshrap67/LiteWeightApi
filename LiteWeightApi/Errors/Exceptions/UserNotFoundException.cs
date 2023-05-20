using LiteWeightAPI.Errors.Attributes.Setup;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;

namespace LiteWeightAPI.Errors.Exceptions;

public class UserNotFoundException : BadRequestException
{
	public UserNotFoundException(string message) : base(GetFormattedResponse(message, ErrorTypes.UserNotFound))
	{
	}
}