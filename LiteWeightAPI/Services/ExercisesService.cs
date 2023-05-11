using AutoMapper;
using LiteWeightAPI.Api.Exercises.Requests;
using LiteWeightAPI.Api.Exercises.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Services.Validation;

namespace LiteWeightAPI.Services;

public interface IExercisesService
{
	Task<OwnedExerciseResponse> CreateExercise(SetExerciseRequest request, string userId);
	Task UpdateExercise(string exerciseId, SetExerciseRequest request, string userId);
	Task DeleteExercise(string exerciseId, string userId);
}

public class ExercisesService : IExercisesService
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;
	private readonly IExercisesValidator _exercisesValidator;

	public ExercisesService(IRepository repository, IMapper mapper, IExercisesValidator exercisesValidator)
	{
		_repository = repository;
		_mapper = mapper;
		_exercisesValidator = exercisesValidator;
	}

	public async Task<OwnedExerciseResponse> CreateExercise(SetExerciseRequest request, string userId)
	{
		var user = await _repository.GetUser(userId);
		_exercisesValidator.ValidCreateExercise(request, user);

		var newExercise = _mapper.Map<OwnedExercise>(request);
		var id = Guid.NewGuid().ToString();
		newExercise.Id = id;
		user.Exercises.Add(newExercise);

		await _repository.PutUser(user);

		var response = _mapper.Map<OwnedExerciseResponse>(newExercise);
		response.Id = id;
		return response;
	}

	public async Task UpdateExercise(string exerciseId, SetExerciseRequest request, string userId)
	{
		var user = await _repository.GetUser(userId);
		_exercisesValidator.ValidUpdateOwnedExercise(exerciseId, request, user);

		var ownedExercise = user.Exercises.First(x => x.Id == exerciseId);
		ownedExercise.Update(request.Name, request.DefaultWeight, request.DefaultSets, request.DefaultReps,
			request.DefaultDetails, request.VideoUrl);

		await _repository.PutUser(user);
	}

	public async Task DeleteExercise(string exerciseId, string userId)
	{
		var user = await _repository.GetUser(userId);
		var ownedExercise = user.Exercises.FirstOrDefault(x => x.Id == exerciseId);
		if (ownedExercise == null)
		{
			return;
		}

		user.Exercises.Remove(ownedExercise);
		await _repository.PutUser(user);
		// can't rely on transactions here since could potentially go over limit (for AWS)
		foreach (var workoutId in ownedExercise.Workouts.Select(x => x.WorkoutId))
		{
			var workout = await _repository.GetWorkout(workoutId);
			workout.Routine.DeleteExerciseFromRoutine(exerciseId);
			await _repository.PutWorkout(workout);
		}
	}
}