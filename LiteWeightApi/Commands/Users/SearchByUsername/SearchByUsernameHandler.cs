using AutoMapper;
using LiteWeightAPI.Api.Users.Responses;
using LiteWeightAPI.Domain;

namespace LiteWeightAPI.Commands.Users.SearchByUsername;

public class SearchByUsernameHandler : ICommandHandler<SearchByUsername, SearchUserResponse>
{
	private readonly IMapper _mapper;
	private readonly IRepository _repository;

	public SearchByUsernameHandler(IRepository repository, IMapper mapper)
	{
		_mapper = mapper;
		_repository = repository;
	}

	public async Task<SearchUserResponse> HandleAsync(SearchByUsername command)
	{
		var user = await _repository.GetUserByUsername(command.Username);

		if (user == null)
		{
			return null;
		}

		// if user is private account, they should not show up in the search unless already friends (or pending friend) with the initiator
		if (user.Preferences.PrivateAccount && user.Friends.All(x => x.UserId != command.InitiatorId))
		{
			return null;
		}

		return _mapper.Map<SearchUserResponse>(user);
	}
}