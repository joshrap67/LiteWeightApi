namespace LiteWeightApi.Api.Common.Responses.ErrorResponses;

public class BadRequestResponse
{
	/// <summary>
	/// Message describing the error.
	/// </summary>
	/// <example>An example error message</example>
	public string Message { get; set; }

	/// <summary>
	/// Specific type of error.
	/// </summary>
	/// <example>InvalidRequest</example>
	public string ErrorType { get; set; }

	/// <summary>
	/// Lists any errors associated when binding the request.
	/// </summary>
	public IEnumerable<ModelBindingError> RequestErrors { get; set; } = new List<ModelBindingError>();
}