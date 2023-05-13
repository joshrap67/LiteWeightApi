using LiteWeightApi.Errors.ErrorAttributes.Setup;
using LiteWeightApi.Errors.Exceptions.BaseExceptions;

namespace LiteWeightApi.Errors.Exceptions;

public class MaxLimitException : BadRequestException
{
	public MaxLimitException(string message) : base(GetFormattedResponse(message, ErrorTypes.MaxLimit))
	{
	}
}