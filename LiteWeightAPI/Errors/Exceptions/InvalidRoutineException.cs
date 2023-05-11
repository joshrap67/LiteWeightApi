using LiteWeightAPI.Errors.ErrorAttributes.Setup;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;

namespace LiteWeightAPI.Errors.Exceptions;

public class InvalidRoutineException : BadRequestException
{
	public InvalidRoutineException(string message) : base(GetFormattedResponse(message, ErrorTypes.InvalidRoutine))
	{
	}
}