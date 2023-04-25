using LiteWeightAPI.Api.Common.Responses.ErrorResponses;
using LiteWeightAPI.Errors.ErrorAttributes.Setup;

namespace LiteWeightAPI.Errors.Exceptions.BaseExceptions;

public class BadRequestException : Exception
{
	public BadRequestResponse FormattedResponse { get; }

	protected BadRequestException(BadRequestResponse formattedResponse) : base(formattedResponse.Message)
	{
		FormattedResponse = formattedResponse;
	}

	protected static BadRequestResponse GetFormattedResponse(string message, string errorType,
		IEnumerable<ModelBindingError> bindingErrors = null)
	{
		return new BadRequestResponse
		{
			Message = message,
			ErrorType = errorType,
			RequestErrors = bindingErrors ?? new List<ModelBindingError>()
		};
	}
}