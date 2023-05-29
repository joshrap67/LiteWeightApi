using AutoMapper;
using LiteWeightAPI.Api.SharedWorkouts.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Commands.SharedWorkouts.GetSharedWorkout;

public class GetSharedWorkoutHandler : ICommandHandler<GetSharedWorkout, SharedWorkoutResponse>
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;

	public GetSharedWorkoutHandler(IRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<SharedWorkoutResponse> HandleAsync(GetSharedWorkout command)
	{
		var sharedWorkout = await _repository.GetSharedWorkout(command.SharedWorkoutId);
		ValidationUtils.SharedWorkoutExists(sharedWorkout);
		ValidationUtils.EnsureSharedWorkoutOwnership(command.UserId, sharedWorkout);

		return _mapper.Map<SharedWorkoutResponse>(sharedWorkout);
	}
}