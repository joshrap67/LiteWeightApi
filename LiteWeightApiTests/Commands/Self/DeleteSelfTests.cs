using AutoFixture;
using LiteWeightAPI.Commands.Self.DeleteSelf;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Services;
using Moq;

namespace LiteWeightApiTests.Commands.Self;

public class DeleteSelfTests
{
	private readonly DeleteSelfHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public DeleteSelfTests()
	{
		_mockRepository = new Mock<IRepository>();
		var storageService = new Mock<IStorageService>().Object;
		var firebaseAuthService = new Mock<IFirebaseAuthService>().Object;
		_handler = new DeleteSelfHandler(_mockRepository.Object, storageService, firebaseAuthService);
	}

	[Fact]
	public async Task Should_Delete_Self()
	{
		var command = _fixture.Create<DeleteSelf>();
		var friendOfUserId = _fixture.Create<string>();
		var userWhoSentFriendRequestId = _fixture.Create<string>();
		var userWhoReceivedFriendRequestId = _fixture.Create<string>();

		var user = _fixture.Build<User>()
			.With(x => x.Friends, new List<Friend>
			{
				_fixture.Build<Friend>()
					.With(x => x.Confirmed, true)
					.With(x => x.UserId, friendOfUserId).Create(),
				_fixture.Build<Friend>()
					.With(x => x.Confirmed, false)
					.With(x => x.UserId, userWhoReceivedFriendRequestId).Create()
			})
			.With(x => x.FriendRequests,
				new List<FriendRequest>
				{
					_fixture.Build<FriendRequest>()
						.With(x => x.UserId, userWhoSentFriendRequestId).Create()
				})
			.With(x=>x.Id, command.UserId)
			.Create();
		var userId = user.Id;

		var friendOfUser = _fixture.Build<User>().With(x => x.Friends, new List<Friend>
		{
			_fixture.Build<Friend>().With(y => y.UserId, userId).Create()
		}).Create();

		var userWhoSentFriendRequest = _fixture.Build<User>().With(x => x.Friends, new List<Friend>
		{
			_fixture.Build<Friend>().With(y => y.UserId, userId).Create()
		}).Create();

		var userWhoReceivedFriendRequest = _fixture.Build<User>().With(x => x.FriendRequests, new List<FriendRequest>
		{
			_fixture.Build<FriendRequest>().With(y => y.UserId, userId).Create()
		}).Create();


		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == userWhoSentFriendRequestId)))
			.ReturnsAsync(userWhoSentFriendRequest);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == friendOfUserId)))
			.ReturnsAsync(friendOfUser);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == userWhoReceivedFriendRequestId)))
			.ReturnsAsync(userWhoReceivedFriendRequest);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);

		Assert.True(friendOfUser.Friends.All(x => x.UserId != userId));
		Assert.True(userWhoSentFriendRequest.Friends.All(x => x.UserId != userId));
		Assert.True(userWhoReceivedFriendRequest.FriendRequests.All(x => x.UserId != userId));
	}

	[Fact]
	public async Task Should_Throw_Exception_User_Does_Not_Exist()
	{
		var command = _fixture.Create<DeleteSelf>();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync((User)null!);

		await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.HandleAsync(command));
	}
}