using AutoMapper;
using LiteWeightAPI.Api.Exercises;
using LiteWeightAPI.Commands.Self.CreateSelf;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Services;

namespace LiteWeightApiTests.Commands.Self;

public class CreateSelfTests
{
	private readonly CreateSelfHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public CreateSelfTests()
	{
		var configuration = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(ExercisesController)); });
		var mapper = new Mapper(configuration);

		_mockRepository = new Mock<IRepository>();
		var storageService = new Mock<IStorageService>().Object;
		_handler = new CreateSelfHandler(_mockRepository.Object, storageService, mapper);
	}

	[Fact]
	public async Task Should_Create_Self()
	{
		var command = _fixture.Create<CreateSelf>();

		_mockRepository
			.Setup(x => x.GetUserByUsername(It.Is<string>(y => y == command.Username)))
			.ReturnsAsync((User)null!);

		_mockRepository
			.Setup(x => x.GetUserByEmail(It.Is<string>(y => y == command.UserEmail)))
			.ReturnsAsync((User)null!);

		var createdUser = await _handler.HandleAsync(command);
		Assert.Equal(command.Username, createdUser.Username);
		Assert.Equal(command.UserEmail, createdUser.Email);
		Assert.Equal(command.UserId, createdUser.Id);
		Assert.Equal(command.MetricUnits, createdUser.Preferences.MetricUnits);
		Assert.NotNull(createdUser.ProfilePicture);
	}

	[Fact]
	public async Task Should_Throw_Exception_Username_Already_Exists()
	{
		var command = _fixture.Create<CreateSelf>();

		var user = _fixture.Build<User>()
			.With(x => x.Username, command.Username)
			.Create();

		_mockRepository
			.Setup(x => x.GetUserByUsername(It.Is<string>(y => y == command.Username)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<AlreadyExistsException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Email_Already_Exists()
	{
		var command = _fixture.Create<CreateSelf>();

		var user = _fixture.Build<User>()
			.With(x => x.Email, command.UserEmail)
			.Create();

		_mockRepository
			.Setup(x => x.GetUserByEmail(It.Is<string>(y => y == command.UserEmail)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<AlreadyExistsException>(() => _handler.HandleAsync(command));
	}
}