using LiteWeightApi.Errors.ErrorAttributes.Setup;
using LiteWeightApi.Errors.Exceptions.BaseExceptions;

namespace LiteWeightApi.Errors.Exceptions;

public class MiscErrorException : BadRequestException
{
	public MiscErrorException(string message) : base(GetFormattedResponse(message, ErrorTypes.MiscError))
	{
	}
}