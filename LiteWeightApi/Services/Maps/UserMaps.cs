using AutoMapper;
using LiteWeightApi.Api.CurrentUser.Responses;
using LiteWeightApi.Api.Exercises.Requests;
using LiteWeightApi.Api.Exercises.Responses;
using LiteWeightApi.Domain.Users;
using LiteWeightApi.Imports;

namespace LiteWeightApi.Services.Maps;

public class UserMaps : Profile
{
	public UserMaps()
	{
		CreateMap<OwnedExercise, OwnedExerciseResponse>();
		CreateMap<OwnedExerciseWorkout, OwnedExerciseWorkoutResponse>();
		CreateMap<WorkoutInfo, WorkoutInfoResponse>();
		CreateMap<Friend, FriendResponse>();
		CreateMap<FriendRequest, FriendRequestResponse>();
		CreateMap<SharedWorkoutInfo, SharedWorkoutInfoResponse>();
		CreateMap<UserPreferences, UserPreferencesResponse>().ReverseMap();
		CreateMap<SetExerciseRequest, OwnedExercise>()
			.Ignore(x => x.Id)
			.Ignore(x => x.Workouts);
		CreateMap<User, UserResponse>();
	}
}