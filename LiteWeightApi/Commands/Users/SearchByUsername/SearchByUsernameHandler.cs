using AutoMapper;
using LiteWeightAPI.Api.Users.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Errors.Exceptions;

namespace LiteWeightAPI.Commands.Users.SearchByUsername;

public class SearchByUsernameHandler : ICommandHandler<SearchByUsername, SearchUserResponse>
{
	private readonly IMapper _mapper;
	private readonly IRepository _repository;

	public SearchByUsernameHandler(IMapper mapper, IRepository repository)
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
			throw new UserNotFoundException($"User {command.Username} not found");
		}

		return _mapper.Map<SearchUserResponse>(user);
	}
}