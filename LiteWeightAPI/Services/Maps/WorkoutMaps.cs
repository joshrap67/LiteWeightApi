using AutoMapper;
using LiteWeightAPI.Api.Workouts.Requests;
using LiteWeightAPI.Api.Workouts.Responses;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightAPI.Services.Maps;

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