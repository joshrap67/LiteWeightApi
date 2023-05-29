using AutoMapper;
using LiteWeightAPI.Api.Exercises;
using LiteWeightAPI.Commands.Users.ReportUser;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using NodaTime;

namespace LiteWeightApiTests.Commands.Users;

public class ReportUserTests
{
	private readonly ReportUserHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public ReportUserTests()
	{
		var configuration = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(ExercisesController)); });
		var mapper = new Mapper(configuration);

		_mockRepository = new Mock<IRepository>();
		var clockMock = new Mock<IClock>().Object;
		_handler = new ReportUserHandler(_mockRepository.Object, clockMock, mapper);
	}

	[Fact]
	public async Task Should_Report_Friend()
	{
		var command = _fixture.Create<ReportUser>();


		var reportedUser = _fixture.Build<User>()
			.With(x => x.Id, command.ReportedUserId)
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.ReportedUserId)))
			.ReturnsAsync(reportedUser);

		var response = await _handler.HandleAsync(command);
		Assert.Equal(command.ReportedUserId, response.ReportedUserId);
	}

	[Fact]
	public async Task Should_Exception_User_Does_Not_Exist()
	{
		var command = _fixture.Create<ReportUser>();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.ReportedUserId)))
			.ReturnsAsync((User)null!);

		await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.HandleAsync(command));
	}
}