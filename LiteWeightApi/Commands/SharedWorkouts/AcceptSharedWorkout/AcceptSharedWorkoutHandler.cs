using AutoMapper;
using LiteWeightAPI.Api.Exercises.Responses;
using LiteWeightAPI.Api.Self.Responses;
using LiteWeightAPI.Api.SharedWorkouts.Responses;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Imports;
using LiteWeightAPI.Utils;
using NodaTime;

namespace LiteWeightAPI.Commands.SharedWorkouts.AcceptSharedWorkout;

public class AcceptSharedWorkoutHandler : ICommandHandler<AcceptSharedWorkout, AcceptSharedWorkoutResponse>
{
	private readonly IRepository _repository;
	private readonly IClock _clock;
	private readonly IMapper _mapper;

	public AcceptSharedWorkoutHandler(IRepository repository, IClock clock, IMapper mapper)
	{
		_repository = repository;
		_clock = clock;
		_mapper = mapper;
	}

	public async Task<AcceptSharedWorkoutResponse> HandleAsync(AcceptSharedWorkout command)
	{
		var user = await _repository.GetUser(command.UserId);
		var workoutToAccept = await _repository.GetSharedWorkout(command.SharedWorkoutId);

		ValidationUtils.SharedWorkoutExists(workoutToAccept);
		ValidationUtils.EnsureSharedWorkoutOwnership(user.Id, workoutToAccept);

		// lots of validation
		var newExercises = GetNewExercisesFromSharedWorkout(workoutToAccept, user).ToList();
		if (user.PremiumToken == null && user.Workouts.Count >= Globals.MaxFreeWorkouts)
		{
			throw new MaxLimitException("Maximum workouts would be exceeded");
		}

		if (user.PremiumToken != null && user.Workouts.Count >= Globals.MaxWorkouts)
		{
			throw new MaxLimitException("Maximum workouts would be exceeded");
		}

		ValidationUtils.ValidWorkoutName(command.NewName ?? workoutToAccept.WorkoutName, user);

		var ownedExerciseNames = user.Exercises.Select(x => x.Name);
		var newExercisesNames = newExercises.Select(x => x.Name);

		var totalExercises = newExercisesNames.Union(ownedExerciseNames).ToList();
		if (user.PremiumToken == null && totalExercises.Count > Globals.MaxFreeExercises)
		{
			throw new MaxLimitException("Accepting this workout would put you above the amount of exercises allowed");
		}

		if (user.PremiumToken != null && totalExercises.Count > Globals.MaxExercises)
		{
			throw new MaxLimitException("Accepting this workout would put you above the amount of exercises allowed");
		}

		if (command.NewName != null)
		{
			workoutToAccept.WorkoutName = command.NewName;
		}

		foreach (var newExercise in newExercises)
		{
			user.Exercises.Add(newExercise);
		}

		var exerciseNameToId = user.Exercises.ToDictionary(x => x.Name, y => y.Id);
		var newWorkoutId = Guid.NewGuid().ToString();
		var now = _clock.GetCurrentInstant();
		var newWorkout = new Workout
		{
			Id = newWorkoutId,
			Name = workoutToAccept.WorkoutName,
			CreatorId = command.UserId,
			CreationUtc = now,
			Routine = new Routine(workoutToAccept.Routine, exerciseNameToId)
		};

		var workoutInfo = new WorkoutInfo
		{
			WorkoutName = newWorkout.Name,
			WorkoutId = newWorkoutId
		};
		user.Workouts.Add(workoutInfo);
		user.ReceivedWorkouts.RemoveAll(x => x.SharedWorkoutId == command.SharedWorkoutId);
		WorkoutUtils.UpdateOwnedExercisesOnCreation(user, newWorkout, false);

		await _repository.ExecuteBatchWrite(
			workoutsToPut: new List<Workout> { newWorkout },
			usersToPut: new List<User> { user },
			sharedWorkoutsToDelete: new List<SharedWorkout> { workoutToAccept }
		);

		return new AcceptSharedWorkoutResponse
		{
			NewWorkoutInfo = _mapper.Map<WorkoutInfoResponse>(workoutInfo),
			UserExercises = _mapper.Map<IList<OwnedExerciseResponse>>(user.Exercises)
		};
	}

	private static IEnumerable<OwnedExercise> GetNewExercisesFromSharedWorkout(SharedWorkout sharedWorkout, User user)
	{
		if (sharedWorkout == null || user == null)
		{
			return new List<OwnedExercise>();
		}

		var newExercises = new List<OwnedExercise>();
		var sharedExerciseNames = sharedWorkout.Routine.Weeks
			.SelectMany(x => x.Days)
			.SelectMany(x => x.Exercises)
			.Select(x => x.ExerciseName)
			.ToHashSet();

		var ownedExerciseNames = user.Exercises.Select(x => x.Name).ToHashSet();

		sharedExerciseNames.ExceptWith(ownedExerciseNames);

		foreach (var exerciseName in sharedExerciseNames)
		{
			// for each of the exercises that the user doesn't own, make a new entry for them
			var sharedExercise = sharedWorkout.DistinctExercises.First(x => x.ExerciseName == exerciseName);
			var ownedExercise = new OwnedExercise
			{
				Name = exerciseName,
				Focuses = sharedExercise.Focuses,
				VideoUrl = sharedExercise.VideoUrl
			};
			newExercises.Add(ownedExercise);
		}

		return newExercises;
	}
}