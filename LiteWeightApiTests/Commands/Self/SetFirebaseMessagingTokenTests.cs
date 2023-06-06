using LiteWeightAPI.Commands.Self.SetFirebaseMessagingToken;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;

namespace LiteWeightApiTests.Commands.Self;

public class SetFirebaseMessagingTokenTests : BaseTest
{
	private readonly SetFirebaseMessagingTokenHandler _handler;
	private readonly Mock<IRepository> _mockRepository;

	public SetFirebaseMessagingTokenTests()
	{
		_mockRepository = new Mock<IRepository>();
		_handler = new SetFirebaseMessagingTokenHandler(_mockRepository.Object);
	}

	[Fact]
	public async Task Should_Set_Current_Workout_Not_Null()
	{
		var command = Fixture.Create<SetFirebaseMessagingToken>();

		var user = Fixture.Build<User>()
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
		var command = Fixture.Build<SetFirebaseMessagingToken>().With(x => x.Token, (string)null).Create();
		var user = Fixture.Build<User>().With(x => x.Id, command.UserId).Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);

		Assert.Null(user.FirebaseMessagingToken);
	}
}