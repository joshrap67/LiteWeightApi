using AutoMapper;
using LiteWeightAPI.Api.Self.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;
using LiteWeightAPI.Services;
using LiteWeightAPI.Services.Notifications;
using LiteWeightAPI.Utils;
using NodaTime;

namespace LiteWeightAPI.Commands.Users.SendFriendRequest;

public class SendFriendRequestHandler : ICommandHandler<SendFriendRequest, FriendResponse>
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;
	private readonly IClock _clock;
	private readonly IPushNotificationService _pushNotificationService;

	public SendFriendRequestHandler(IRepository repository, IMapper mapper, IClock clock,
		IPushNotificationService pushNotificationService)
	{
		_repository = repository;
		_mapper = mapper;
		_clock = clock;
		_pushNotificationService = pushNotificationService;
	}

	public async Task<FriendResponse> HandleAsync(SendFriendRequest command)
	{
		var recipientId = command.RecipientId;
		var senderId = command.SenderId;

		var senderUser = await _repository.GetUser(senderId);
		var recipientUser = await _repository.GetUser(recipientId);

		// validation
		CommonValidator.UserExists(recipientUser);
		var senderUserId = senderUser.Id;

		if (recipientUser.Preferences.PrivateAccount && recipientUser.Friends.All(x => x.UserId != senderUserId))
		{
			throw new ResourceNotFoundException("User");
		}

		if (senderUser.Friends.Count >= Globals.MaxNumberFriends)
		{
			throw new MaxLimitException("Max number of friends would be exceeded");
		}

		if (recipientUser.FriendRequests.Count >= Globals.MaxFriendRequests)
		{
			throw new MaxLimitException($"{recipientId} has too many pending requests");
		}

		if (senderUser.FriendRequests.Any(x => x.UserId == recipientId))
		{
			throw new MiscErrorException("This user has already sent you a friend request");
		}

		if (senderUserId.Equals(senderUserId, StringComparison.InvariantCultureIgnoreCase))
		{
			throw new MiscErrorException("Cannot send a friend request to yourself");
		}

		// if already sent, fail gracefully
		var existingFriend = senderUser.Friends.FirstOrDefault(x => x.UserId == recipientId);
		if (existingFriend != null)
		{
			return _mapper.Map<FriendResponse>(existingFriend);
		}

		var friendToAdd = new Friend
		{
			Username = recipientUser.Username,
			UserId = recipientUser.Id,
			ProfilePicture = recipientUser.ProfilePicture
		};
		var now = _clock.GetCurrentInstant();
		var friendRequest = new FriendRequest
		{
			UserId = senderUserId,
			Username = senderUser.Username,
			ProfilePicture = senderUser.ProfilePicture,
			SentUtc = now
		};
		senderUser.Friends.Add(friendToAdd);
		recipientUser.FriendRequests.Add(friendRequest);

		await _repository.ExecuteBatchWrite(usersToPut: new List<User> { senderUser, recipientUser });

		// send a notification to the user that received the friend request
		await _pushNotificationService.SendNewFriendRequestNotification(recipientUser, friendRequest);

		return _mapper.Map<FriendResponse>(friendToAdd);
	}
}