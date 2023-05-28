using LiteWeightAPI.Commands.Self.SetFirebaseToken;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;

namespace LiteWeightApiTests.Commands.Self;

public class SetFirebaseTokenTests
{
	private readonly SetFirebaseTokenHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public SetFirebaseTokenTests()
	{
		_mockRepository = new Mock<IRepository>();
		_handler = new SetFirebaseTokenHandler(_mockRepository.Object);
	}

	[Fact]
	public async Task Should_Set_Current_Workout_Not_Null()
	{
		var command = _fixture.Create<SetFirebaseToken>();

		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);

		Assert.Equal(command.Token, user.FirebaseMessagingToken);
	}

	[Fact]
	public async Task Should_Set_Current_Workout_Null()
	{
		var command = _fixture.Build<SetFirebaseToken>().With(x => x.Token, () => null!).Create();
		var user = _fixture.Build<User>().With(x => x.Id, command.UserId).Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);

		Assert.Null(user.FirebaseMessagingToken);
	}
}