using AutoFixture;
using LiteWeightAPI.Commands.Self.SetAllReceivedWorkoutsSeen;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using Moq;

namespace LiteWeightApiTests.Commands.Self;

public class SetAllReceivedWorkoutsSeenTests
{
	private readonly SetAllReceivedWorkoutsSeenHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public SetAllReceivedWorkoutsSeenTests()
	{
		_mockRepository = new Mock<IRepository>();
		_handler = new SetAllReceivedWorkoutsSeenHandler(_mockRepository.Object);
	}

	[Fact]
	public async Task Should_Set_Workouts_Seen()
	{
		var command = _fixture.Create<SetAllReceivedWorkoutsSeen>();
		var user = _fixture.Build<User>()
			.With(x => x.ReceivedWorkouts,
				new List<SharedWorkoutInfo>
				{
					_fixture.Build<SharedWorkoutInfo>().With(x => x.Seen, false).Create(),
					_fixture.Build<SharedWorkoutInfo>().With(x => x.Seen, false).Create(),
					_fixture.Build<SharedWorkoutInfo>().With(x => x.Seen, false).Create(),
					_fixture.Build<SharedWorkoutInfo>().With(x => x.Seen, true).Create()
				})
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);
		Assert.True(user.ReceivedWorkouts.All(x => x.Seen));
	}

	[Fact]
	public async Task Should_Set_Workouts_Seen_Empty_List()
	{
		var command = _fixture.Create<SetAllReceivedWorkoutsSeen>();
		var user = _fixture.Build<User>()
			.With(x => x.ReceivedWorkouts, new List<SharedWorkoutInfo>())
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);
		Assert.True(user.ReceivedWorkouts.All(x => x.Seen));
	}
}