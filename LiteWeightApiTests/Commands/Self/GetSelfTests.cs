using AutoMapper;
using LiteWeightAPI.Api.Exercises;
using LiteWeightAPI.Commands.Self.GetSelf;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;

namespace LiteWeightApiTests.Commands.Self;

public class GetSelfTests
{
	private readonly GetSelfHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public GetSelfTests()
	{
		var configuration = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(ExercisesController)); });
		var mapper = new Mapper(configuration);

		_mockRepository = new Mock<IRepository>();
		_handler = new GetSelfHandler(_mockRepository.Object, mapper);
	}

	[Fact]
	public async Task Should_Get_Self()
	{
		var command = _fixture.Create<GetSelf>();

		var user = _fixture.Build<User>().With(x => x.Id, command.UserId).Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		var response = await _handler.HandleAsync(command);

		Assert.Equal(command.UserId, response.Id);
	}

	[Fact]
	public async Task Should_Throw_Exception_User_Does_Not_Exist()
	{
		var command = _fixture.Create<GetSelf>();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync((User)null!);

		await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.HandleAsync(command));
	}
}