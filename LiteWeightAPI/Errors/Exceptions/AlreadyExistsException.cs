using LiteWeightAPI.Api.Common.Responses.ErrorResponses;
using LiteWeightAPI.Errors.ErrorAttributes;
using LiteWeightAPI.Errors.ErrorAttributes.Setup;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;

namespace LiteWeightAPI.Errors.Exceptions;

public class AlreadyExistsException : BadRequestException
{
	public AlreadyExistsException(string message) : base(GetFormattedResponse(message, ErrorTypes.AlreadyExists))
	{
	}
}