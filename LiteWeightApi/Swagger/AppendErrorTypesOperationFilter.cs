using LiteWeightAPI.Errors.Attributes.Setup;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LiteWeightAPI.Swagger;

public class AppendErrorTypesOperationFilter : IOperationFilter
{
	/// <summary>
	/// Automatically appends error types to the end of an operation description if the controller has any error attributes
	/// </summary>
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		context.ApiDescription.TryGetMethodInfo(out var methodInfo);
		if (methodInfo == null)
		{
			return;
		}

		var attributes =
			(IEnumerable<BaseErrorAttribute>)Attribute.GetCustomAttributes(methodInfo, typeof(BaseErrorAttribute));

		var errors = attributes
			.Select(x => x.ErrorType)
			.ToList();
		if (!errors.Any())
		{
			return;
		}

		errors.Sort(StringComparer.InvariantCulture);
		operation.Description +=
			$"<br/><br/>" +
			$"<div style=\"background-color: #2f3031;padding: 12px 0px 12px 12px;border-left: 5px solid #F90258;\">" +
			$"Potential 400 Errors: <b>{string.Join(", ", errors)}</b>" +
			$"</div>";
	}
}