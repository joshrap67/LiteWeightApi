using LiteWeightApi.Errors.ErrorAttributes.Setup;
using LiteWeightApi.Errors.Exceptions.BaseExceptions;

namespace LiteWeightApi.Errors.Exceptions;

public class AlreadyExistsException : BadRequestException
{
	public AlreadyExistsException(string message) : base(GetFormattedResponse(message, ErrorTypes.AlreadyExists))
	{
	}
}