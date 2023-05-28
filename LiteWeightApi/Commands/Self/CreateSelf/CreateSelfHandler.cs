using AutoMapper;
using LiteWeightAPI.Api.Self.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Imports;
using LiteWeightAPI.Services;

namespace LiteWeightAPI.Commands.Self.CreateSelf;

public class CreateSelfHandler : ICommandHandler<CreateSelf, UserResponse>
{
	private readonly IRepository _repository;
	private readonly IStorageService _storageService;
	private readonly IMapper _mapper;

	public CreateSelfHandler(IRepository repository, IStorageService storageService, IMapper mapper)
	{
		_repository = repository;
		_storageService = storageService;
		_mapper = mapper;
	}

	public async Task<UserResponse> HandleAsync(CreateSelf command)
	{
		var userByUsername = await _repository.GetUserByUsername(command.Username);
		if (userByUsername != null)
		{
			throw new AlreadyExistsException("User already exists with this username");
		}

		// todo test
		var userByEmail = await _repository.GetUserByEmail(command.UserEmail);
		if (userByEmail != null)
		{
			throw new AlreadyExistsException("There is already an account associated with this email");
		}

		// whenever a user is created, give them a unique UUID file path that will always get updated
		var fileName = Guid.NewGuid().ToString();
		if (command.ProfilePictureData != null)
		{
			await _storageService.UploadProfilePicture(command.ProfilePictureData, fileName);
		}
		else
		{
			await _storageService.UploadDefaultProfilePicture(fileName);
		}

		var userPreferences = new UserPreferences
		{
			MetricUnits = command.MetricUnits,
			PrivateAccount = false,
			UpdateDefaultWeightOnRestart = true,
			UpdateDefaultWeightOnSave = true
		};
		var user = new User
		{
			Id = command.UserId,
			Email = command.UserEmail,
			ProfilePicture = fileName,
			Username = command.Username,
			Preferences = userPreferences,
			Exercises = Defaults.GetDefaultExercises()
		};
		await _repository.CreateUser(user);

		var retVal = _mapper.Map<UserResponse>(user);
		return retVal;
	}
}