using LiteWeightApi.Errors.ErrorAttributes.Setup;
using LiteWeightApi.Errors.Exceptions.BaseExceptions;

namespace LiteWeightApi.Errors.Exceptions;

public class WorkoutNotFoundException : BadRequestException
{
	public WorkoutNotFoundException(string message) : base(GetFormattedResponse(message, ErrorTypes.WorkoutNotFound))
	{
	}
}