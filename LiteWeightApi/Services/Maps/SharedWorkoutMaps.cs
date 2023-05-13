using AutoMapper;
using LiteWeightApi.Api.SharedWorkouts.Responses;
using LiteWeightApi.Domain.SharedWorkouts;

namespace LiteWeightApi.Services.Maps;

public class SharedWorkoutMaps : Profile
{
	public SharedWorkoutMaps()
	{
		CreateMap<SharedWorkout, SharedWorkoutResponse>();
		CreateMap<SharedWorkoutDistinctExercise, SharedWorkoutDistinctExerciseResponse>();
		CreateMap<SharedRoutine, SharedRoutineResponse>();
		CreateMap<SharedWeek, SharedWeekResponse>();
		CreateMap<SharedDay, SharedDayResponse>();
		CreateMap<SharedExercise, SharedExerciseResponse>();
	}
}