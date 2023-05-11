using AutoMapper;
using LiteWeightAPI.Api.CurrentUser.Responses;
using LiteWeightAPI.Api.Exercises.Requests;
using LiteWeightAPI.Api.Exercises.Responses;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Services.Maps;

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