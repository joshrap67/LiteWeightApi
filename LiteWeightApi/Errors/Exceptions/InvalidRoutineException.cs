using LiteWeightApi.Errors.ErrorAttributes.Setup;
using LiteWeightApi.Errors.Exceptions.BaseExceptions;

namespace LiteWeightApi.Errors.Exceptions;

public class InvalidRoutineException : BadRequestException
{
	public InvalidRoutineException(string message) : base(GetFormattedResponse(message, ErrorTypes.InvalidRoutine))
	{
	}
}