using AutoMapper;
using LiteWeightAPI.Api.Users.Responses;
using LiteWeightAPI.Domain.Complaints;

namespace LiteWeightAPI.Services.Maps;

public class ReportMaps : Profile
{
	public ReportMaps()
	{
		CreateMap<Complaint, ComplaintResponse>();
	}
}