using AutoMapper;
using LiteWeightApi.Api.Workouts.Requests;
using LiteWeightApi.Api.Workouts.Responses;
using LiteWeightApi.Domain.Workouts;

namespace LiteWeightApi.Services.Maps;

public class WorkoutMaps : Profile
{
	public WorkoutMaps()
	{
		CreateMap<SetRoutineRequest, Routine>();
		CreateMap<SetRoutineWeekRequest, RoutineWeek>();
		CreateMap<SetRoutineDayRequest, RoutineDay>();
		CreateMap<SetRoutineExerciseRequest, RoutineExercise>();

		CreateMap<Workout, WorkoutResponse>();
		CreateMap<Routine, RoutineResponse>();
		CreateMap<RoutineWeek, RoutineWeekResponse>();
		CreateMap<RoutineDay, RoutineDayResponse>();
		CreateMap<RoutineExercise, RoutineExerciseResponse>();
	}
}