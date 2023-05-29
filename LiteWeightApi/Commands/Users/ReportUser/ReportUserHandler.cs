using AutoMapper;
using LiteWeightAPI.Api.Users.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Complaints;
using LiteWeightAPI.Utils;
using NodaTime;

namespace LiteWeightAPI.Commands.Users.ReportUser;

public class ReportUserHandler : ICommandHandler<ReportUser, ComplaintResponse>
{
	private readonly IRepository _repository;
	private readonly IClock _clock;
	private readonly IMapper _mapper;

	public ReportUserHandler(IRepository repository, IClock clock, IMapper mapper)
	{
		_repository = repository;
		_clock = clock;
		_mapper = mapper;
	}

	public async Task<ComplaintResponse> HandleAsync(ReportUser command)
	{
		var userToReport = await _repository.GetUser(command.ReportedUserId);

		ValidationUtils.UserExists(userToReport);

		var complaint = new Complaint
		{
			ReportedUserId = command.ReportedUserId,
			ReportedUsername = userToReport.Username,
			ReportedUtc = _clock.GetCurrentInstant(),
			ClaimantUserId = command.InitiatorUserId
		};
		await _repository.CreateComplaint(complaint);

		return _mapper.Map<ComplaintResponse>(complaint);
	}
}