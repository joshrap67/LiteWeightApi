using AutoMapper;
using LiteWeightAPI.Api.Self.Responses;
using LiteWeightAPI.Api.Workouts.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Utils;

namespace LiteWeightAPI.Commands.Workouts.UpdateRoutine;

public class UpdateRoutineHandler : ICommandHandler<UpdateRoutine, UserAndWorkoutResponse>
{
	private readonly IRepository _repository;
	private readonly IMapper _mapper;

	public UpdateRoutineHandler(IRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<UserAndWorkoutResponse> HandleAsync(UpdateRoutine command)
	{
		var user = await _repository.GetUser(command.UserId);
		var workout = await _repository.GetWorkout(command.WorkoutId);
		var routine = _mapper.Map<Routine>(command.Routine);

		ValidationUtils.WorkoutExists(workout);
		ValidationUtils.EnsureWorkoutOwnership(user.Id, workout);
		ValidationUtils.ValidRoutine(routine);

		UpdateOwnedExercisesOnEdit(user, routine, workout);
		workout.Routine = routine;
		WorkoutUtils.FixCurrentDayAndWeek(workout, user.Workouts.First(x => x.WorkoutId == command.WorkoutId));

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

	private static void UpdateOwnedExercisesOnEdit(User user, Routine newRoutine, Workout workout)
	{
		var updateDefaultWeight = user.Settings.UpdateDefaultWeightOnSave;
		var currentExerciseIds = new HashSet<string>();
		var exerciseIdToExercise = user.Exercises.ToDictionary(x => x.Id, x => x);
		foreach (var week in newRoutine.Weeks)
		{
			foreach (var day in week.Days)
			{
				foreach (var routineExercise in day.Exercises)
				{
					var exerciseId = routineExercise.ExerciseId;
					var ownedExercise = exerciseIdToExercise[exerciseId];
					if (updateDefaultWeight && routineExercise.Weight > ownedExercise.DefaultWeight)
					{
						ownedExercise.DefaultWeight = routineExercise.Weight;
					}

					currentExerciseIds.Add(exerciseId);
				}
			}
		}

		var oldExerciseIds = new HashSet<string>();
		foreach (var week in workout.Routine.Weeks)
		{
			foreach (var day in week.Days)
			{
				foreach (var routineExercise in day.Exercises)
				{
					oldExerciseIds.Add(routineExercise.ExerciseId);
				}
			}
		}

		var deletedExercises = oldExerciseIds.Where(x => !currentExerciseIds.Contains(x)).ToList();
		var newExercises = currentExerciseIds.Where(x => !oldExerciseIds.Contains(x)).ToList();

		foreach (var ownedExercise in newExercises.Select(exerciseId => exerciseIdToExercise[exerciseId]))
		{
			ownedExercise.Workouts.Add(new OwnedExerciseWorkout
			{
				WorkoutId = workout.Id,
				WorkoutName = workout.Name
			});
		}

		foreach (var exerciseId in deletedExercises)
		{
			var ownedExercise = exerciseIdToExercise[exerciseId];
			var ownedExerciseWorkout = ownedExercise.Workouts.First(x => x.WorkoutId == workout.Id);
			ownedExercise.Workouts.Remove(ownedExerciseWorkout);
		}
	}
}