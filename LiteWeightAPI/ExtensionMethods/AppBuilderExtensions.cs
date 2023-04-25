using System.Text.Json;
using LiteWeightAPI.Api.Common.Responses.ErrorResponses;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace LiteWeightAPI.ExtensionMethods;

public static class AppBuilderExtensions
{
    public static void UseCustomExceptionHandling(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseExceptionHandler(builder =>
        {
            // whenever an exception is thrown and not caught, it ends up here to be formatted properly and returned
            builder.Run(async context =>
            {
                var httpContextException = context.Features.Get<IExceptionHandlerFeature>();
                var exception = httpContextException?.Error;
                var serializeOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                context.Response.ContentType = "application/json";

                switch (exception)
                {
                    case BadRequestException bre:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(bre.FormattedResponse, serializeOptions));
                        break;
                    case UnauthorizedException ue:
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync(
                            JsonSerializer.Serialize(new UnauthorizedResponse { Message = ue.Message }, serializeOptions));
                        break;
                    case ForbiddenException fe:
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync(
                            JsonSerializer.Serialize(new ForbiddenResponse { Message = fe.Message }, serializeOptions));
                        break;
                    case ResourceNotFoundException rnfe:
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(
                            new ResourceNotFoundResponse { Message = rnfe.FormattedMessage }, serializeOptions));
                        break;
                    case UpgradeRequiredException ure:
                        context.Response.StatusCode = StatusCodes.Status426UpgradeRequired;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(
                            new ResourceNotFoundResponse { Message = ure.Message }, serializeOptions));
                        break;
                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(
                            new ServerErrorResponse { Message = "The server has encountered an error" }, serializeOptions));
                        break;
                }
            });
        });
    }
}