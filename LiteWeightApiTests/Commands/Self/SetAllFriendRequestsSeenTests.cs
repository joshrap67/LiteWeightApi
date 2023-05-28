using AutoFixture;
using LiteWeightAPI.Commands.Self.SetAllFriendRequestsSeen;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using Moq;

namespace LiteWeightApiTests.Commands.Self;

public class SetAllFriendRequestsSeenTests
{
	private readonly SetAllFriendRequestsSeenHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public SetAllFriendRequestsSeenTests()
	{
		_mockRepository = new Mock<IRepository>();
		_handler = new SetAllFriendRequestsSeenHandler(_mockRepository.Object);
	}

	[Fact]
	public async Task Should_Set_Requests_Seen()
	{
		var command = _fixture.Create<SetAllFriendRequestsSeen>();
		var user = _fixture.Build<User>()
			.With(x => x.FriendRequests,
				new List<FriendRequest>
				{
					_fixture.Build<FriendRequest>().With(x => x.Seen, false).Create(),
					_fixture.Build<FriendRequest>().With(x => x.Seen, false).Create(),
					_fixture.Build<FriendRequest>().With(x => x.Seen, false).Create(),
					_fixture.Build<FriendRequest>().With(x => x.Seen, true).Create()
				})
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);
		Assert.True(user.FriendRequests.All(x => x.Seen));
	}

	[Fact]
	public async Task Should_Set_Requests_Seen_Empty_List()
	{
		var command = _fixture.Create<SetAllFriendRequestsSeen>();
		var user = _fixture.Build<User>()
			.With(x => x.FriendRequests, new List<FriendRequest>())
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);
		Assert.True(user.FriendRequests.All(x => x.Seen));
	}
}