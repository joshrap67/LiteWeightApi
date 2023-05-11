using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace LiteWeightAPI.Api.System;

[Route("system")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class SystemController : BaseController
{
	public SystemController(ILogger logger) : base(logger)
	{
	}

	[HttpPut("submit-feedback")]
	public async Task<ActionResult> SubmitFeedback()
	{
		return Ok();
	}
}