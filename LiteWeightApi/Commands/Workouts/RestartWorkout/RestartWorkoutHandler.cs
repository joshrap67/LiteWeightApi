using AutoMapper;
using LiteWeightAPI.Api.Self.Responses;
using LiteWeightAPI.Api.Workouts.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Commands.Workouts.RestartWorkout;

public class RestartWorkoutHandler : ICommandHandler<RestartWorkout, UserAndWorkoutResponse>
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;

	public RestartWorkoutHandler(IRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<UserAndWorkoutResponse> HandleAsync(RestartWorkout command)
	{
		var user = await _repository.GetUser(command.UserId);
		var workout = await _repository.GetWorkout(command.WorkoutId);
		var routine = _mapper.Map<Routine>(command.Routine);

		ValidationUtils.WorkoutExists(workout);
		ValidationUtils.EnsureWorkoutOwnership(user.Id, workout);

		var workoutInfo = user.Workouts.First(x => x.WorkoutId == command.WorkoutId);
		RestartWorkout(routine, workoutInfo, user);
		workout.Routine = routine;
		workoutInfo.TimesRestarted += 1;
		workoutInfo.CurrentDay = 0;
		workoutInfo.CurrentWeek = 0;

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { workout },
			usersToPut: new List<User> { user }
		);

		return new UserAndWorkoutResponse
		{
			User = _mapper.Map<UserResponse>(user),
			Workout = _mapper.Map<WorkoutResponse>(workout)
		};
	}

	private static void RestartWorkout(Routine routine, WorkoutInfo workoutInfo, User user)
	{
		var exerciseIdToExercise = user.Exercises.ToDictionary(x => x.Id, x => x);
		foreach (var week in routine.Weeks)
		{
			foreach (var day in week.Days)
			{
				foreach (var routineExercise in day.Exercises)
				{
					if (routineExercise.Completed)
					{
						workoutInfo.AverageExercisesCompleted =
							IncreaseAverage(workoutInfo.AverageExercisesCompleted, workoutInfo.TotalExercisesSum, 1);
						routineExercise.Completed = false;

						if (user.Settings.UpdateDefaultWeightOnRestart)
						{
							// automatically update default weight with this weight if it's higher than previous
							var exerciseId = routineExercise.ExerciseId;
							var ownedExercise = exerciseIdToExercise[exerciseId];
							if (routineExercise.Weight > ownedExercise.DefaultWeight)
							{
								ownedExercise.DefaultWeight = routineExercise.Weight;
							}
						}
					}
					else
					{
						// didn't complete the exercise, still need to update new average with this 0 value
						workoutInfo.AverageExercisesCompleted =
							IncreaseAverage(workoutInfo.AverageExercisesCompleted, workoutInfo.TotalExercisesSum, 0);
					}

					workoutInfo.TotalExercisesSum += 1;
				}
			}
		}
	}

	private static double IncreaseAverage(double oldAverage, int count, double newValue)
	{
		return ((newValue + (oldAverage * count)) / (count + 1));
	}
}