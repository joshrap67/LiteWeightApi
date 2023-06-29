using AutoMapper;
using LiteWeightAPI.Api.ReceivedWorkouts.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Commands.ReceivedWorkouts.GetReceivedWorkout;

public class GetReceivedWorkoutHandler : ICommandHandler<GetReceivedWorkout, ReceivedWorkoutResponse>
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;

	public GetReceivedWorkoutHandler(IRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<ReceivedWorkoutResponse> HandleAsync(GetReceivedWorkout command)
	{
		var receivedWorkout = await _repository.GetReceivedWorkout(command.ReceivedWorkoutId);
		ValidationUtils.ReceivedWorkoutExists(receivedWorkout);
		ValidationUtils.EnsureReceivedWorkoutOwnership(command.UserId, receivedWorkout);

		return _mapper.Map<ReceivedWorkoutResponse>(receivedWorkout);
	}
}