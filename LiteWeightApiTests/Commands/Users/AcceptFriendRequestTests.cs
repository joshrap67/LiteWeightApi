using LiteWeightAPI.Commands.Users.AcceptFriendRequest;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;
using LiteWeightAPI.Services;

namespace LiteWeightApiTests.Commands.Users;

public class AcceptFriendRequestTests
{
	private readonly AcceptFriendRequestHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public AcceptFriendRequestTests()
	{
		_mockRepository = new Mock<IRepository>();
		var pushNotificationService = new Mock<IPushNotificationService>().Object;
		_handler = new AcceptFriendRequestHandler(_mockRepository.Object, pushNotificationService);
	}

	[Fact]
	public async Task Should_Accept_Friend_Request()
	{
		var command = _fixture.Create<AcceptFriendRequest>();
		var friends = Enumerable.Range(0, Globals.MaxNumberFriends / 2)
			.Select(_ => _fixture.Build<Friend>().Create())
			.ToList();
		var friendRequests = new List<FriendRequest>
		{
			_fixture.Create<FriendRequest>(),
			_fixture.Build<FriendRequest>().With(x => x.UserId, command.AcceptedUserId).Create()
		};

		var initiator = _fixture.Build<User>()
			.With(x => x.Id, command.InitiatorUserId)
			.With(x => x.Friends, friends)
			.With(x => x.FriendRequests, friendRequests)
			.Create();
		var acceptedUser = _fixture.Build<User>()
			.With(x => x.Id, command.AcceptedUserId)
			.With(x => x.Friends, new List<Friend>
			{
				_fixture.Build<Friend>()
					.With(x => x.UserId, command.InitiatorUserId)
					.With(x => x.Confirmed, false)
					.Create()
			})
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.AcceptedUserId)))
			.ReturnsAsync(acceptedUser);
		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.InitiatorUserId)))
			.ReturnsAsync(initiator);

		var response = await _handler.HandleAsync(command);
		Assert.True(response);
		Assert.True(initiator.FriendRequests.All(x => x.UserId != command.AcceptedUserId));
		Assert.Contains(initiator.Friends, x => x.UserId == command.AcceptedUserId && x.Confirmed);
		Assert.Contains(acceptedUser.Friends, x => x.UserId == command.InitiatorUserId && x.Confirmed);
	}

	[Fact]
	public async Task Should_Not_Fail_Friend_Request_Not_Found()
	{
		var command = _fixture.Create<AcceptFriendRequest>();
		var friends = Enumerable.Range(0, Globals.MaxNumberFriends / 2)
			.Select(_ => _fixture.Build<Friend>().Create())
			.ToList();

		var initiator = _fixture.Build<User>()
			.With(x => x.Id, command.InitiatorUserId)
			.With(x => x.Friends, friends)
			.Create();
		var acceptedUser = _fixture.Build<User>().With(x => x.Id, command.AcceptedUserId).Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.AcceptedUserId)))
			.ReturnsAsync(acceptedUser);
		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.InitiatorUserId)))
			.ReturnsAsync(initiator);

		var response = await _handler.HandleAsync(command);
		Assert.False(response);
	}

	[Fact]
	public async Task Should_Exception_User_Does_Not_Exist()
	{
		var command = _fixture.Create<AcceptFriendRequest>();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.AcceptedUserId)))
			.ReturnsAsync((User)null!);

		await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Exception_Initiator_Max_Friends()
	{
		var command = _fixture.Create<AcceptFriendRequest>();
		var friends = Enumerable.Range(0, Globals.MaxNumberFriends + 1)
			.Select(_ => _fixture.Build<Friend>().Create())
			.ToList();

		var initiator = _fixture.Build<User>()
			.With(x => x.Id, command.InitiatorUserId)
			.With(x => x.Friends, friends)
			.Create();
		var acceptedUser = _fixture.Build<User>().With(x => x.Id, command.AcceptedUserId).Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.AcceptedUserId)))
			.ReturnsAsync(acceptedUser);
		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.InitiatorUserId)))
			.ReturnsAsync(initiator);

		await Assert.ThrowsAsync<MaxLimitException>(() => _handler.HandleAsync(command));
	}
}