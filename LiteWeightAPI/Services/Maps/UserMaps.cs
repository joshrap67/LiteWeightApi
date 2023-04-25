using AutoMapper;
using LiteWeightAPI.Api.Users.Requests;
using LiteWeightAPI.Api.Users.Responses;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Services.Maps;

public class UserMaps : Profile
{
	public UserMaps()
	{
		CreateMap<OwnedExercise, OwnedExerciseResponse>();
		CreateMap<OwnedExerciseWorkout, OwnedExerciseWorkoutResponse>();
		CreateMap<WorkoutMeta, WorkoutMetaResponse>();
		CreateMap<Friend, FriendResponse>();
		CreateMap<Blocked, BlockedUserResponse>();
		CreateMap<FriendRequest, FriendRequestResponse>();
		CreateMap<SharedWorkoutMeta, SharedWorkoutMetaResponse>();
		CreateMap<UserPreferences, UserPreferencesResponse>().ReverseMap();
		CreateMap<CreateExerciseRequest, OwnedExercise>()
			.Ignore(x => x.Id)
			.Ignore(x => x.Workouts);
		CreateMap<UpdateExerciseRequest, OwnedExercise>()
			.Ignore(x => x.Id)
			.Ignore(x => x.Workouts);
		CreateMap<User, UserResponse>();
	}
}