using LiteWeightAPI.Api.Users.Responses;

namespace LiteWeightAPI.Commands.Users.ReportUser;

public class ReportUser : ICommand<ComplaintResponse>
{
	public string InitiatorUserId { get; set; }
	public string ReportedUserId { get; set; }
	public string Description { get; set; }
}