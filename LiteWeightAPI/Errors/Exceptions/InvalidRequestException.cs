using LiteWeightAPI.Api.Common.Responses.ErrorResponses;
using LiteWeightAPI.Errors.ErrorAttributes;
using LiteWeightAPI.Errors.ErrorAttributes.Setup;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;

namespace LiteWeightAPI.Errors.Exceptions;

public class InvalidRequestException : BadRequestException
{
	public InvalidRequestException(string message, IEnumerable<ModelBindingError> bindingErrors = null) :
		base(GetFormattedResponse(message, ErrorTypes.InvalidRequest, bindingErrors))
	{
	}
}