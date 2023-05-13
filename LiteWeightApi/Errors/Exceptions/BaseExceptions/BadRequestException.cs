using LiteWeightApi.Api.Common.Responses.ErrorResponses;

namespace LiteWeightApi.Errors.Exceptions.BaseExceptions;

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