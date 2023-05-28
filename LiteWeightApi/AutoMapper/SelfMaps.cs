using AutoMapper;
using LiteWeightAPI.Api.Self.Requests;
using LiteWeightAPI.Api.Self.Responses;
using LiteWeightAPI.Commands.Self.CreateSelf;
using LiteWeightAPI.Commands.Self.SetPreferences;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.ExtensionMethods;

namespace LiteWeightAPI.AutoMapper;

public class SelfMaps : Profile
{
	public SelfMaps()
	{
		CreateMap<WorkoutInfo, WorkoutInfoResponse>();
		CreateMap<Friend, FriendResponse>();
		CreateMap<FriendRequest, FriendRequestResponse>();
		CreateMap<SharedWorkoutInfo, SharedWorkoutInfoResponse>();
		CreateMap<UserPreferences, UserPreferencesResponse>();
		CreateMap<User, UserResponse>();
		
		CreateMap<CreateUserRequest, CreateSelf>().Ignore(x => x.UserEmail).Ignore(x => x.UserId);
		CreateMap<UserPreferencesResponse, SetPreferences>().Ignore(x => x.UserId);
	}
}