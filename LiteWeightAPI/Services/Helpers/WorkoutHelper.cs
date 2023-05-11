using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightAPI.Services.Helpers;

public static class WorkoutHelper
{
	public static void UpdateOwnedExercisesOnCreation(User user, Workout newWorkout)
	{
		var updateDefaultWeight = user.UserPreferences.UpdateDefaultWeightOnSave;
		var exerciseIds = new HashSet<string>();
		var exerciseIdToExercise = user.Exercises.ToDictionary(x => x.Id, x => x);
		foreach (var week in newWorkout.Routine.Weeks)
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

					exerciseIds.Add(exerciseId);
				}
			}
		}

		foreach (var exerciseId in exerciseIds)
		{
			exerciseIdToExercise[exerciseId].Workouts.Add(new OwnedExerciseWorkout
			{
				WorkoutId = newWorkout.Id,
				WorkoutName = newWorkout.Name
			});
		}
	}

	public static void UpdateOwnedExercisesOnEdit(User user, Routine newRoutine, Workout workout)
	{
		var updateDefaultWeight = user.UserPreferences.UpdateDefaultWeightOnSave;
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

		foreach (var exerciseId in newExercises)
		{
			var ownedExercise = exerciseIdToExercise[exerciseId];
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

	public static void FixCurrentDayAndWeek(Workout editedWorkout)
	{
		// make sure that the current week according to the request is actually valid
		var currentDay = editedWorkout.CurrentDay;
		var currentWeek = editedWorkout.CurrentWeek;
		if (currentWeek < 0 && currentWeek >= editedWorkout.Routine.Weeks.Count)
		{
			// request incorrectly set the current week, so just set both to 0
			editedWorkout.CurrentWeek = 0;
			editedWorkout.CurrentDay = 0;
			return;
		}

		if (currentDay < 0 && currentDay >= editedWorkout.Routine.Weeks[currentWeek].Days.Count)
		{
			// request incorrectly set the current day, so just set it to 0
			editedWorkout.CurrentWeek = 0;
			editedWorkout.CurrentDay = 0;
		}
	}

	/// <summary>
	/// Reset each exercise in the workout to be not completed, and update statistics and default weights where necessary.
	/// </summary>
	public static void RestartWorkout(Routine routine, WorkoutInfo workoutInfo, User user)
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

						if (user.UserPreferences.UpdateDefaultWeightOnRestart)
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

	public static string FindMostFrequentFocus(User user, Routine routine)
	{
		var exerciseIdToExercise = user.Exercises.ToDictionary(x => x.Id, x => x);
		var focusCount = new Dictionary<string, int>();
		foreach (var week in routine.Weeks)
		{
			foreach (var day in week.Days)
			{
				foreach (var routineExercise in day.Exercises)
				{
					var exerciseId = routineExercise.ExerciseId;
					foreach (var focus in exerciseIdToExercise[exerciseId].Focuses)
					{
						if (!focusCount.ContainsKey(focus))
						{
							focusCount[focus] = 1;
						}
						else
						{
							focusCount[focus]++;
						}
					}
				}
			}
		}

		var max = focusCount.Values.Max();

		var maxFocuses = (from focus in focusCount.Keys
				let count = focusCount[focus]
				where count == max
				select focus)
			.ToList();

		var retVal = string.Join(",", maxFocuses);
		return retVal;
	}
}