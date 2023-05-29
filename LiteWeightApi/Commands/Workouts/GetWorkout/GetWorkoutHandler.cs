using AutoMapper;
using LiteWeightAPI.Api.Workouts.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Commands.Workouts.GetWorkout;

public class GetWorkoutHandler : ICommandHandler<GetWorkout, WorkoutResponse>
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;

	public GetWorkoutHandler(IRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<WorkoutResponse> HandleAsync(GetWorkout command)
	{
		var workout = await _repository.GetWorkout(command.WorkoutId);

		ValidationUtils.WorkoutExists(workout);
		ValidationUtils.EnsureWorkoutOwnership(command.UserId, workout);

		return _mapper.Map<WorkoutResponse>(workout);
	}
}