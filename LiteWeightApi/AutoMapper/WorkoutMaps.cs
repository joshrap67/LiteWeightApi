using AutoMapper;
using LiteWeightAPI.Api.Workouts.Requests;
using LiteWeightAPI.Api.Workouts.Responses;
using LiteWeightAPI.Commands.Workouts;
using LiteWeightAPI.Commands.Workouts.CreateWorkout;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.ExtensionMethods;

namespace LiteWeightAPI.AutoMapper;

public class WorkoutMaps : Profile
{
	public WorkoutMaps()
	{
		CreateMap<CreateWorkoutRequest, CreateWorkout>().Ignore(x => x.UserId);
		CreateMap<SetRoutineRequest, SetRoutine>();
		CreateMap<SetRoutineWeekRequest, SetRoutineWeek>();
		CreateMap<SetRoutineDayRequest, SetRoutineDay>();
		CreateMap<SetRoutineExerciseRequest, SetRoutineExercise>();

		CreateMap<Workout, WorkoutResponse>();
		CreateMap<Routine, RoutineResponse>();
		CreateMap<RoutineWeek, RoutineWeekResponse>();
		CreateMap<RoutineDay, RoutineDayResponse>();
		CreateMap<RoutineExercise, RoutineExerciseResponse>();
	}
}