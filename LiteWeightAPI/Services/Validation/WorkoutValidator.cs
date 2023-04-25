using LiteWeightAPI.Api.Workouts.Requests;
using LiteWeightAPI.Api.Workouts.Responses;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Services.Validation;

public interface IWorkoutValidator
{
	void EnsureWorkoutOwnership(string username, Workout workout);
	void EnsureValidNewWorkout(CreateWorkoutRequest request, User activeUser);
	void EnsureValidEditWorkout(SetRoutineRequest request);
	void EnsureValidCopyWorkout(CopyWorkoutRequest request, User activeUser);
}

public class WorkoutWorkoutValidator : IWorkoutValidator
{
	private readonly ICommonValidator _commonValidator;

	public WorkoutWorkoutValidator(ICommonValidator commonValidator)
	{
		_commonValidator = commonValidator;
	}
	
	public void EnsureWorkoutOwnership(string username, Workout workout)
	{
		if (workout.Creator != username)
		{
			throw new ForbiddenException("User does not have permissions to modify workout");
		}
	}

	public void EnsureValidNewWorkout(CreateWorkoutRequest request, User activeUser)
	{
		if (activeUser.Workouts.Count >= Globals.MaxFreeWorkouts && activeUser.PremiumToken == null)
		{
			throw new Exception("Max amount of free workouts reached");
		}

		if (activeUser.Workouts.Count >= Globals.MaxWorkouts && activeUser.PremiumToken != null)
		{
			throw new Exception("Maximum workouts exceeded");
		}

		_commonValidator.EnsureValidWorkoutName(request.WorkoutName, activeUser);
		EnsureValidRoutineWeeks(request.Routine);
		EnsureValidRoutineDays(request.Routine);
	}

	public void EnsureValidEditWorkout(SetRoutineRequest request)
	{
		EnsureValidRoutineWeeks(request.Routine);
		EnsureValidRoutineDays(request.Routine);
	}

	public void EnsureValidCopyWorkout(CopyWorkoutRequest request, User activeUser)
	{
		if (activeUser.Workouts.Count >= Globals.MaxFreeWorkouts && activeUser.PremiumToken == null)
		{
			throw new Exception("Max amount of free workouts reached");
		}

		if (activeUser.Workouts.Count >= Globals.MaxWorkouts && activeUser.PremiumToken != null)
		{
			throw new Exception("Maximum workouts exceeded");
		}

		_commonValidator.EnsureValidWorkoutName(request.NewName, activeUser);
	}

	private static void EnsureValidRoutineDays(RoutineResponse routine)
	{
		var weekIndex = 0;
		foreach (var week in routine.Weeks)
		{
			weekIndex++;
			var dayCount = week.Days.Count;
			if (dayCount > Globals.MaxDaysRoutine)
			{
				throw new Exception($"Week: {weekIndex} exceeds maximum amount of days in a week");
			}

			var dayIndex = 0;
			foreach (var day in week.Days)
			{
				dayIndex++;
				if (day.Tag != null && day.Tag.Length > Globals.MaxDayTagLength)
				{
					throw new Exception($"Day tag for Week: {weekIndex} Day: {dayIndex} exceeds maximum length");
				}
			}
		}
	}

	private static void EnsureValidRoutineWeeks(RoutineResponse routine)
	{
		if (routine.Weeks.Count > Globals.MaxWeeksRoutine)
		{
			throw new Exception("Workout exceeds maximum amount of weeks allowed");
		}
	}
}