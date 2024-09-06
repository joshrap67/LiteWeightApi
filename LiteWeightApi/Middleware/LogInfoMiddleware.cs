using LiteWeightAPI.Imports;
using ILogger = Serilog.ILogger;

namespace LiteWeightAPI.Middleware;

public class LogInfoMiddleware(RequestDelegate next, ILogger logger)
{
	public async Task InvokeAsync(HttpContext context)
	{
		var version = context.Request.Headers[RequestFields.VersionNameHeader].ToString();
		var versionCodeString = context.Request.Headers[RequestFields.AndroidVersionCodeHeader].ToString();
		logger.Information($"LiteWeight version code for request: {version}");
		logger.Information($"LiteWeight android version number for request: {versionCodeString}");

		await next(context);
	}
}