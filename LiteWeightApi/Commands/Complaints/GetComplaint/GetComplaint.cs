using LiteWeightAPI.Api.Complaints.Responses;

namespace LiteWeightAPI.Commands.Complaints.GetComplaint;

public class GetComplaint : ICommand<ComplaintResponse>
{
	public required string UserId { get; set; }
	public required string ComplaintId { get; set; }
}