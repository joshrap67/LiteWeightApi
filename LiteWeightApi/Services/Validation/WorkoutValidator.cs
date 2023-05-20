using LiteWeightAPI.Api.Workouts.Requests;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Services.Validation;

public interface IWorkoutValidator
{
	void ValidGetWorkout(Workout workout, string userId);
	void ValidCreateWorkout(CreateWorkoutRequest request, User user);
	void ValidSetRoutine(Workout workout, SetRoutineRequest request, User user);
	void ValidUpdateProgress(Workout workout, UpdateWorkoutProgressRequest updateWorkoutProgressRequest, User user);
	void ValidRestartWorkout(Workout workout, User user);
	void ValidDeleteWorkout(Workout workout, User user);
	void ValidCopyWorkout(CopyWorkoutRequest request, Workout workoutToCopy, User user);
	void ValidRenameWorkout(RenameWorkoutRequest request, Workout workout, User user);
	void ValidResetStatistics(User user, string workoutId);
}

public class WorkoutWorkoutValidator : IWorkoutValidator
{
	private readonly ICommonValidator _commonValidator;

	public WorkoutWorkoutValidator(ICommonValidator commonValidator)
	{
		_commonValidator = commonValidator;
	}

	public void ValidGetWorkout(Workout workout, string userId)
	{
		if (workout == null)
		{
			throw new ResourceNotFoundException("Workout");
		}

		_commonValidator.EnsureWorkoutOwnership(userId, workout);
	}

	public void ValidCreateWorkout(CreateWorkoutRequest request, User user)
	{
		if (user.Workouts.Count >= Globals.MaxFreeWorkouts && user.PremiumToken == null)
		{
			throw new MaxLimitException("Max amount of free workouts reached");
		}

		if (user.Workouts.Count >= Globals.MaxWorkouts && user.PremiumToken != null)
		{
			throw new MaxLimitException("Maximum workouts exceeded");
		}

		_commonValidator.ValidWorkoutName(request.WorkoutName, user);
		ValidRoutineWeeks(request.Routine);
		ValidRoutineDays(request.Routine);
	}

	public void ValidSetRoutine(Workout workout, SetRoutineRequest request, User user)
	{
		_commonValidator.EnsureWorkoutOwnership(user.Id, workout);
		_commonValidator.WorkoutExists(workout);
		ValidRoutineWeeks(request);
		ValidRoutineDays(request);
	}

	public void ValidUpdateProgress(Workout workout, UpdateWorkoutProgressRequest request, User user)
	{
		_commonValidator.WorkoutExists(workout);
		_commonValidator.EnsureWorkoutOwnership(user.Id, workout);
		ValidRoutineWeeks(request.Routine);
		ValidRoutineDays(request.Routine);
	}

	public void ValidRestartWorkout(Workout workout, User user)
	{
		_commonValidator.WorkoutExists(workout);
		_commonValidator.EnsureWorkoutOwnership(user.Id, workout);
	}

	public void ValidDeleteWorkout(Workout workout, User user)
	{
		_commonValidator.WorkoutExists(workout);
		_commonValidator.EnsureWorkoutOwnership(user.Id, workout);
	}

	public void ValidCopyWorkout(CopyWorkoutRequest request, Workout workoutToCopy, User user)
	{
		_commonValidator.WorkoutExists(workoutToCopy);
		_commonValidator.EnsureWorkoutOwnership(user.Id, workoutToCopy);

		if (user.Workouts.Count >= Globals.MaxFreeWorkouts && user.PremiumToken == null)
		{
			throw new MaxLimitException("Max amount of free workouts reached");
		}

		if (user.Workouts.Count >= Globals.MaxWorkouts && user.PremiumToken != null)
		{
			throw new MaxLimitException("Maximum workouts exceeded");
		}

		_commonValidator.ValidWorkoutName(request.NewName, user);
	}

	public void ValidRenameWorkout(RenameWorkoutRequest request, Workout workout, User user)
	{
		_commonValidator.WorkoutExists(workout);
		_commonValidator.EnsureWorkoutOwnership(user.Id, workout);
		_commonValidator.ValidWorkoutName(request.NewName, user);
	}

	public void ValidResetStatistics(User user, string workoutId)
	{
		if (user.Workouts.All(x => x.WorkoutId != workoutId))
		{
			throw new ResourceNotFoundException("Workout");
		}
	}

	private static void ValidRoutineDays(SetRoutineRequest routine)
	{
		var weekIndex = 0;
		foreach (var week in routine.Weeks)
		{
			weekIndex++;
			var dayCount = week.Days.Count;
			if (dayCount > Globals.MaxDaysRoutine)
			{
				throw new InvalidRoutineException($"Week: {weekIndex} exceeds maximum amount of days in a week");
			}

			var dayIndex = 0;
			foreach (var day in week.Days)
			{
				dayIndex++;
				if (day.Tag != null && day.Tag.Length > Globals.MaxDayTagLength)
				{
					throw new InvalidRoutineException(
						$"Day tag for Week: {weekIndex} Day: {dayIndex} exceeds maximum length");
				}
			}
		}
	}

	private static void ValidRoutineWeeks(SetRoutineRequest routine)
	{
		if (routine.Weeks.Count > Globals.MaxWeeksRoutine)
		{
			throw new InvalidRoutineException("Workout exceeds maximum amount of weeks allowed");
		}
	}
}