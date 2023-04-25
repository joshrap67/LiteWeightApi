using LiteWeightAPI.Errors.ErrorAttributes.Setup;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;

namespace LiteWeightAPI.Errors.Exceptions;

public class MaximumReachedException : BadRequestException
{
	public MaximumReachedException(string message) : base(GetFormattedResponse(message, ErrorTypes.MaximumReached))
	{
	}
}