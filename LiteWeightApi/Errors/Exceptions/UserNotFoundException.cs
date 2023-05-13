using LiteWeightApi.Errors.ErrorAttributes.Setup;
using LiteWeightApi.Errors.Exceptions.BaseExceptions;

namespace LiteWeightApi.Errors.Exceptions;

public class UserNotFoundException : BadRequestException
{
	public UserNotFoundException(string message) : base(GetFormattedResponse(message, ErrorTypes.UserNotFound))
	{
	}
}