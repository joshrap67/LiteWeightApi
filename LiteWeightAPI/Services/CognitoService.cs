using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Services;

public interface ICognitoService
{
	Task DeleteUser(string userId);
}

public class CognitoService : ICognitoService
{
	private readonly AmazonCognitoIdentityProviderClient _provider;

	public CognitoService()
	{
		_provider = new AmazonCognitoIdentityProviderClient(RegionEndpoint.USEast1);
	}

	public async Task DeleteUser(string userId)
	{
		await _provider.AdminDeleteUserAsync(new AdminDeleteUserRequest
		{
			Username = userId,
			UserPoolId = Globals.CognitoUserPool
		});
	}
}