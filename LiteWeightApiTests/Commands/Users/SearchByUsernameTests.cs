using LiteWeightAPI.Commands.Users.SearchByUsername;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightApiTests.TestHelpers;

namespace LiteWeightApiTests.Commands.Users;

public class SearchByUsernameTests
{
	private readonly SearchByUsernameHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public SearchByUsernameTests()
	{
		_mockRepository = new Mock<IRepository>();
		_handler = new SearchByUsernameHandler(_mockRepository.Object, DependencyHelper.GetMapper());
	}

	[Fact]
	public async Task Should_Find_User_Not_Private_Account()
	{
		var command = _fixture.Create<SearchByUsername>();

		var preferences = new UserSettings();
		var foundUser = _fixture.Build<User>()
			.With(x => x.Username, command.Username)
			.With(x => x.Settings, preferences)
			.Create();

		_mockRepository
			.Setup(x => x.GetUserByUsername(It.Is<string>(y => y == command.Username)))
			.ReturnsAsync(foundUser);

		var response = await _handler.HandleAsync(command);
		Assert.NotNull(response);
		Assert.Equal(command.Username, response.Username);
	}

	[Fact]
	public async Task Should_Find_User_Private_Account()
	{
		var command = _fixture.Create<SearchByUsername>();

		var preferences = new UserSettings { PrivateAccount = true };
		var foundUser = _fixture.Build<User>()
			.With(x => x.Username, command.Username)
			.With(x => x.Settings, preferences)
			.With(x => x.Friends, new List<Friend>
			{
				_fixture.Build<Friend>().With(x => x.UserId, command.InitiatorId).Create()
			})
			.Create();

		_mockRepository
			.Setup(x => x.GetUserByUsername(It.Is<string>(y => y == command.Username)))
			.ReturnsAsync(foundUser);

		var response = await _handler.HandleAsync(command);
		Assert.NotNull(response);
		Assert.Equal(command.Username, response.Username);
	}

	[Fact]
	public async Task Should_Return_Null_User_Not_Found()
	{
		var command = _fixture.Create<SearchByUsername>();

		_mockRepository
			.Setup(x => x.GetUserByUsername(It.Is<string>(y => y == command.Username)))
			.ReturnsAsync((User)null);

		var response = await _handler.HandleAsync(command);
		Assert.Null(response);
	}

	[Fact]
	public async Task Should_Return_Null_Private_User()
	{
		var command = _fixture.Create<SearchByUsername>();

		var preferences = new UserSettings { PrivateAccount = true };
		var foundUser = _fixture.Build<User>()
			.With(x => x.Username, command.Username)
			.With(x => x.Settings, preferences)
			.Create();

		_mockRepository
			.Setup(x => x.GetUserByUsername(It.Is<string>(y => y == command.Username)))
			.ReturnsAsync(foundUser);

		var response = await _handler.HandleAsync(command);
		Assert.Null(response);
	}
}