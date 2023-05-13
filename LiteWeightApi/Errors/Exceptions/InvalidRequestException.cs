using LiteWeightApi.Api.Common.Responses.ErrorResponses;
using LiteWeightApi.Errors.ErrorAttributes.Setup;
using LiteWeightApi.Errors.Exceptions.BaseExceptions;

namespace LiteWeightApi.Errors.Exceptions;

public class InvalidRequestException : BadRequestException
{
	public InvalidRequestException(string message, IEnumerable<ModelBindingError> bindingErrors = null) :
		base(GetFormattedResponse(message, ErrorTypes.InvalidRequest, bindingErrors))
	{
	}
}