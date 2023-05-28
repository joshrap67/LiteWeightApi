using AutoMapper;
using LiteWeightAPI.Api.SharedWorkouts.Responses;
using LiteWeightAPI.Domain.SharedWorkouts;

namespace LiteWeightAPI.AutoMapper;

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