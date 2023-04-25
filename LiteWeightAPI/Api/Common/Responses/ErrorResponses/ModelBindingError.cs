﻿namespace LiteWeightAPI.Api.Common.Responses.ErrorResponses;

public class ModelBindingError
{
	/// <summary>
	/// Property that is causing the error.
	/// </summary>
	/// <example>name</example>
	public string Property { get; init; }

	/// <summary>
	/// Error message for problematic property.
	/// </summary>
	/// <example>The name property cannot be empty</example>
	public string Message { get; init; }
}