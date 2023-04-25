using LiteWeightAPI.Api.Common.Responses.ErrorResponses;
using LiteWeightAPI.Errors.ErrorAttributes;
using LiteWeightAPI.Errors.ErrorAttributes.Setup;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;

namespace LiteWeightAPI.Errors.Exceptions;

public class DuplicateFoundException : BadRequestException
{
	public DuplicateFoundException(string message) : base(GetFormattedResponse(message, ErrorTypes.DuplicateFound))
	{
	}
}