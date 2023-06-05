using System.Text.Json.Nodes;
using LiteWeightAPI.Imports;
using LiteWeightAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LiteWeightAPI.Api;

[Produces("application/json")]
[Authorize]
public class BaseController : Controller
{
	private readonly Serilog.ILogger _logger;
	protected string CurrentUserId { get; private set; }
	protected string CurrentUserEmail { get; private set; }

	public BaseController(Serilog.ILogger logger)
	{
		_logger = logger;
	}

	public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		try
		{
			var version = HttpContext.Request.Headers[RequestFields.VersionNameHeader].ToString();
			var versionCodeString = HttpContext.Request.Headers[RequestFields.AndroidVersionCodeHeader].ToString();
			_logger.Information($"LiteWeight version code for request: {version}");
			_logger.Information($"LiteWeight android version number for request: {versionCodeString}");
		}
		catch (Exception)
		{
			_logger.Information("Error getting version codes. Continuing request...");
		}

		var firebaseClaim = HttpContext.User.Claims.ToList().FirstOrDefault(x => x.Type == "firebase");
		if (firebaseClaim != null)
		{
			var deserializedToken = JsonUtils.Deserialize<JsonNode>(firebaseClaim.Value);
			var email = deserializedToken["identities"]?["email"]?[0]?.GetValue<string>();
			CurrentUserEmail = email;
		}

		var userIdClaim = HttpContext.User.Claims.ToList().FirstOrDefault(x => x.Type == "user_id");
		CurrentUserId = userIdClaim?.Value;

		return base.OnActionExecutionAsync(context, next);
	}
}