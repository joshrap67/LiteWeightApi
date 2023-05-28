using AutoFixture;
using LiteWeightAPI.Commands.Self.SetReceivedWorkoutSeen;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using Moq;

namespace LiteWeightApiTests.Commands.Self;

public class SetReceivedWorkoutSeenTests
{
	private readonly SetReceivedWorkoutSeenHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public SetReceivedWorkoutSeenTests()
	{
		_mockRepository = new Mock<IRepository>();
		_handler = new SetReceivedWorkoutSeenHandler(_mockRepository.Object);
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task Should_Set_Workout_Seen(bool isAlreadySeen)
	{
		var command = _fixture.Create<SetReceivedWorkoutSeen>();

		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.Create();
		var sharedWorkoutInfo = _fixture.Build<SharedWorkoutInfo>()
			.With(x => x.SharedWorkoutId, command.SharedWorkoutId)
			.With(x => x.Seen, isAlreadySeen)
			.Create();
		user.ReceivedWorkouts.Add(sharedWorkoutInfo);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);
		Assert.True(sharedWorkoutInfo.Seen);
	}

	[Fact]
	public async Task Should_Not_Fail_Workout_Not_Found()
	{
		var command = _fixture.Create<SetReceivedWorkoutSeen>();

		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		var response = await _handler.HandleAsync(command);
		Assert.False(response);
	}
}