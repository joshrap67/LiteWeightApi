using LiteWeightAPI.Api.Complaints.Responses;

namespace LiteWeightAPI.Commands.Users.ReportUser;

public class ReportUser : ICommand<ComplaintResponse>
{
	public required string InitiatorUserId { get; set; }
	public required string ReportedUserId { get; set; }
	public required string Description { get; set; }
}