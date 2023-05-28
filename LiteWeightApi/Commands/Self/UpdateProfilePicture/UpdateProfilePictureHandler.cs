using LiteWeightAPI.Domain;
using LiteWeightAPI.Services;

namespace LiteWeightAPI.Commands.Self.UpdateProfilePicture;

public class UpdateProfilePictureHandler : ICommandHandler<UpdateProfilePicture, bool>
{
	private readonly IRepository _repository;
	private readonly IStorageService _storageService;

	public UpdateProfilePictureHandler(IRepository repository, IStorageService storageService)
	{
		_repository = repository;
		_storageService = storageService;
	}

	public async Task<bool> HandleAsync(UpdateProfilePicture command)
	{
		var user = await _repository.GetUser(command.UserId);
		await _storageService.UploadProfilePicture(command.ImageData, user.ProfilePicture);
		return true;
	}
}