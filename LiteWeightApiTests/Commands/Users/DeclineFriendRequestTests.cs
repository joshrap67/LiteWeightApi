using LiteWeightAPI.Commands.Users.DeclineFriendRequest;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Services;

namespace LiteWeightApiTests.Commands.Users;

public class DeclineFriendRequestTests
{
	private readonly DeclineFriendRequestHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public DeclineFriendRequestTests()
	{
		_mockRepository = new Mock<IRepository>();
		var pushNotificationService = new Mock<IPushNotificationService>().Object;
		_handler = new DeclineFriendRequestHandler(_mockRepository.Object, pushNotificationService);
	}

	[Fact]
	public async Task Should_Decline_Friend_Request()
	{
		var command = _fixture.Create<DeclineFriendRequest>();
		var friendRequests = new List<FriendRequest>
		{
			_fixture.Create<FriendRequest>(),
			_fixture.Build<FriendRequest>().With(x => x.UserId, command.UserIdToDecline).Create()
		};

		var initiator = _fixture.Build<User>()
			.With(x => x.Id, command.InitiatorUserId)
			.With(x => x.FriendRequests, friendRequests)
			.Create();
		var declinedUser = _fixture.Build<User>()
			.With(x => x.Id, command.UserIdToDecline)
			.With(x => x.Friends, new List<Friend>
			{
				_fixture.Build<Friend>()
					.With(x => x.UserId, command.InitiatorUserId)
					.With(x => x.Confirmed, false)
					.Create()
			})
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserIdToDecline)))
			.ReturnsAsync(declinedUser);
		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.InitiatorUserId)))
			.ReturnsAsync(initiator);

		var response = await _handler.HandleAsync(command);
		Assert.True(response);
		Assert.True(initiator.FriendRequests.All(x => x.UserId != command.UserIdToDecline));
		Assert.True(declinedUser.Friends.All(x => x.UserId != command.InitiatorUserId));
	}

	[Fact]
	public async Task Should_Not_Fail_Friend_Request_Not_Found()
	{
		var command = _fixture.Create<DeclineFriendRequest>();

		var initiator = _fixture.Build<User>()
			.With(x => x.Id, command.InitiatorUserId)
			.Create();
		var declinedUser = _fixture.Build<User>().With(x => x.Id, command.UserIdToDecline).Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserIdToDecline)))
			.ReturnsAsync(declinedUser);
		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.InitiatorUserId)))
			.ReturnsAsync(initiator);

		var response = await _handler.HandleAsync(command);
		Assert.False(response);
	}

	[Fact]
	public async Task Should_Exception_User_Does_Not_Exist()
	{
		var command = _fixture.Create<DeclineFriendRequest>();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserIdToDecline)))
			.ReturnsAsync((User)null!);

		await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.HandleAsync(command));
	}
}