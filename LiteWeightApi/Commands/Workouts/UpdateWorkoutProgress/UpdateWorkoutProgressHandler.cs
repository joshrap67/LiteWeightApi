using AutoMapper;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Commands.Workouts.UpdateWorkoutProgress;

public class UpdateWorkoutProgressHandler : ICommandHandler<UpdateWorkoutProgress, bool>
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;

	public UpdateWorkoutProgressHandler(IRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<bool> HandleAsync(UpdateWorkoutProgress command)
	{
		var user = await _repository.GetUser(command.UserId);
		var workoutToUpdate = await _repository.GetWorkout(command.WorkoutId);
		var routine = _mapper.Map<Routine>(command.Routine);

		ValidationUtils.WorkoutExists(workoutToUpdate);
		ValidationUtils.EnsureWorkoutOwnership(user.Id, workoutToUpdate);
		ValidationUtils.ValidRoutine(routine);

		workoutToUpdate.Routine = routine;
		workoutToUpdate.CurrentWeek = command.CurrentWeek;
		workoutToUpdate.CurrentDay = command.CurrentDay;

		WorkoutUtils.FixCurrentDayAndWeek(workoutToUpdate);

		await _repository.PutWorkout(workoutToUpdate);

		return true;
	}
}