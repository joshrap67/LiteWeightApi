using System.Text.Json;
using System.Text.Json.Nodes;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
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

	private const int MinimumLiteWeightAndroidVersion = 11; // update this when there is a breaking change

	public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		int? versionCode = null;

		try
		{
			var version = HttpContext.Request.Headers[RequestFields.VersionNameHeader].ToString();
			var versionCodeString = HttpContext.Request.Headers[RequestFields.AndroidVersionCodeHeader].ToString();
			versionCode = int.Parse(versionCodeString);
			_logger.Information($"LiteWeight version code for request: {version}");
		}
		catch (Exception)
		{
			_logger.Error("Version code not in proper format. Continuing request...");
		}


		if (versionCode is < MinimumLiteWeightAndroidVersion)
		{
			throw new UpgradeRequiredException(
				$"The minimum LiteWeight Android version required to use this API is {MinimumLiteWeightAndroidVersion}");
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