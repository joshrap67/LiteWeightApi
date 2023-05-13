using LiteWeightApi.Api.Common.Responses.ErrorResponses;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LiteWeightApi.Swagger;

public class DefaultResponsesOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		const string contentType = "application/json";
		// ensure every operation in swagger spec has these types of responses since every operation can return these
		operation.Responses[StatusCodes.Status401Unauthorized.ToString()] =
			new OpenApiResponse
			{
				Description = "Authentication Error",
				Content = new Dictionary<string, OpenApiMediaType>
				{
					[contentType] = new()
					{
						Schema = context.SchemaGenerator.GenerateSchema(typeof(UnauthorizedResponse),
							context.SchemaRepository)
					}
				}
			};

		operation.Responses[StatusCodes.Status403Forbidden.ToString()] =
			new OpenApiResponse
			{
				Description = "Authorization Error",
				Content = new Dictionary<string, OpenApiMediaType>
				{
					[contentType] = new()
					{
						Schema = context.SchemaGenerator.GenerateSchema(typeof(ForbiddenResponse),
							context.SchemaRepository)
					}
				}
			};

		operation.Responses[StatusCodes.Status426UpgradeRequired.ToString()] =
			new OpenApiResponse
			{
				Description = "Current LiteWeight version needs to be updated",
				Content = new Dictionary<string, OpenApiMediaType>
				{
					[contentType] = new()
					{
						Schema = context.SchemaGenerator.GenerateSchema(typeof(UpgradeRequiredResponse),
							context.SchemaRepository)
					}
				}
			};

		operation.Responses[StatusCodes.Status500InternalServerError.ToString()] =
			new OpenApiResponse
			{
				Description = "Server Error",
				Content = new Dictionary<string, OpenApiMediaType>
				{
					[contentType] = new()
					{
						Schema = context.SchemaGenerator.GenerateSchema(typeof(ServerErrorResponse),
							context.SchemaRepository)
					}
				}
			};
	}
}