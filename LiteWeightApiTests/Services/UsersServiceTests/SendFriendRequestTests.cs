using AutoFixture;
using LiteWeightAPI.Domain.Users;

namespace LiteWeightApiTests.Services.UsersServiceTests;

public class SendFriendRequestTests
{
	private readonly IFixture _fixture;
	
	public SendFriendRequestTests()
	{
		_fixture = new Fixture();
	}
	
	[Fact]
	public void Success()
	{
		var recipientUser = _fixture.Build<User>().Create();
		var senderUser = _fixture.Build<User>().Create();
		
	}
}