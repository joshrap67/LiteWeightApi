using LiteWeightAPI.Commands.Self.SetPreferences;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;

namespace LiteWeightApiTests.Commands.Self;

public class SetPreferencesTests
{
	private readonly SetPreferencesHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public SetPreferencesTests()
	{
		_mockRepository = new Mock<IRepository>();
		_handler = new SetPreferencesHandler(_mockRepository.Object);
	}

	[Fact]
	public async Task Should_Set_Preferences()
	{
		var command = _fixture.Create<SetPreferences>();

		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);

		Assert.Equal(command.MetricUnits, user.Preferences.MetricUnits);
		Assert.Equal(command.PrivateAccount, user.Preferences.PrivateAccount);
		Assert.Equal(command.UpdateDefaultWeightOnRestart, user.Preferences.UpdateDefaultWeightOnRestart);
		Assert.Equal(command.UpdateDefaultWeightOnSave, user.Preferences.UpdateDefaultWeightOnSave);
	}
}