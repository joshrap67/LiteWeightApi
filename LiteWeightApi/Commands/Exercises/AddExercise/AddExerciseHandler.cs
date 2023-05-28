using AutoMapper;
using LiteWeightAPI.Api.Exercises.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Commands.Exercises.AddExercise;

public class AddExerciseHandler : ICommandHandler<AddExercise, OwnedExerciseResponse>
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;

	public AddExerciseHandler(IRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<OwnedExerciseResponse> HandleAsync(AddExercise command)
	{
		var user = await _repository.GetUser(command.UserId);

		var exerciseNames = user.Exercises.Select(x => x.Name);

		if (exerciseNames.Any(x => x == command.Name))
		{
			throw new AlreadyExistsException("Exercise name already exists");
		}

		if (user.PremiumToken == null && user.Exercises.Count >= Globals.MaxFreeExercises)
		{
			throw new MaxLimitException("Max exercise limit reached");
		}

		if (user.PremiumToken != null && user.Exercises.Count >= Globals.MaxExercises)
		{
			throw new MaxLimitException("Max exercise limit reached");
		}

		var newExercise = _mapper.Map<OwnedExercise>(command);
		user.Exercises.Add(newExercise);

		await _repository.PutUser(user);

		var response = _mapper.Map<OwnedExerciseResponse>(newExercise);
		response.Id = newExercise.Id;
		return response;
	}
}