using LiteWeightAPI.Commands.Self.UpdateProfilePicture;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Services;

namespace LiteWeightApiTests.Commands.Self;

public class UpdateProfilePictureTests
{
	private readonly UpdateProfilePictureHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public UpdateProfilePictureTests()
	{
		_mockRepository = new Mock<IRepository>();
		var storageService = new Mock<IStorageService>();
		_handler = new UpdateProfilePictureHandler(_mockRepository.Object, storageService.Object);
	}

	[Fact]
	public async Task Should_Set_Profile_Picture()
	{
		var command = _fixture.Create<UpdateProfilePicture>();

		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		var response = await _handler.HandleAsync(command);
		Assert.True(response);
	}
}