using LiteWeightAPI.Api.Complaints.Responses;
using LiteWeightAPI.Commands;
using LiteWeightAPI.Commands.Complaints.GetComplaint;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace LiteWeightAPI.Api.Complaints;

[Route("complaints")]
[ApiController]
public class ComplaintsController : BaseController
{
	private readonly ICommandDispatcher _dispatcher;

	public ComplaintsController(ILogger logger, ICommandDispatcher dispatcher) : base(logger)
	{
		_dispatcher = dispatcher;
	}

	/// <summary>Get Complaint</summary>
	/// <remarks>Gets a complaint that was filed by the authenticated user.</remarks>
	[HttpGet("{complaintId}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ComplaintResponse>> GetComplaint(string complaintId)
	{
		var command = new GetComplaint { ComplaintId = complaintId, UserId = CurrentUserId };
		var response = await _dispatcher.DispatchAsync<GetComplaint, ComplaintResponse>(command);

		return response;
	}
}