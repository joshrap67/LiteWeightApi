using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Utils;

public static class CommonValidator
{
	public static void UserExists(User user)
	{
		if (user == null)
		{
			throw new ResourceNotFoundException("User");
		}
	}

	public static void WorkoutExists(Workout workout)
	{
		if (workout == null)
		{
			throw new ResourceNotFoundException("Workout");
		}
	}

	public static void ReferencedWorkoutExists(Workout workout)
	{
		if (workout == null)
		{
			throw new WorkoutNotFoundException("Referenced workout does not exist");
		}
	}

	public static void EnsureWorkoutOwnership(string userId, Workout workout)
	{
		if (workout.CreatorId != userId)
		{
			throw new ForbiddenException("User does not have permissions to access workout");
		}
	}

	public static void EnsureSharedWorkoutOwnership(string userId, SharedWorkout sharedWorkout)
	{
		if (sharedWorkout.RecipientId != userId)
		{
			throw new ForbiddenException("User does not have permissions to access shared workout");
		}
	}

	public static void SharedWorkoutExists(SharedWorkout workout)
	{
		if (workout == null)
		{
			throw new ResourceNotFoundException("Shared workout");
		}
	}

	public static void ValidWorkoutName(string workoutName, User user)
	{
		if (user.Workouts.Any(x => x.WorkoutName == workoutName))
		{
			throw new AlreadyExistsException("Workout name already exists");
		}
	}

	public static void ValidRoutine(Routine routine)
	{
		if (routine.Weeks.Count > Globals.MaxWeeksRoutine)
		{
			throw new InvalidRoutineException("Workout exceeds maximum amount of weeks allowed");
		}

		// check days
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
}