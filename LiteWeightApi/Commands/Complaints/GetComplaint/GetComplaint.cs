using LiteWeightAPI.Api.Complaints.Responses;

namespace LiteWeightAPI.Commands.Complaints.GetComplaint;

public class GetComplaint : ICommand<ComplaintResponse>
{
	public string UserId { get; set; }
	public string ComplaintId { get; set; }
}