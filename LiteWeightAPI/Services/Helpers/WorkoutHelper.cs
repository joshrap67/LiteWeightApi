using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightAPI.Services.Helpers;

public static class WorkoutHelper
{
	public static void UpdateOwnedExercisesOnCreation(User user, Workout newWorkout)
	{
		var updateDefaultWeight = user.UserPreferences.UpdateDefaultWeightOnSave;
		var exerciseIds = new List<string>();
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
						// if user wants to update default weight on save and this exercise has a greater
						// weight than the current default, then update the default
						ownedExercise.DefaultWeight = routineExercise.Weight;
					}

					exerciseIds.Add(exerciseId);
				}
			}
		}

		foreach (var exerciseId in exerciseIds)
		{
			// updates the list of exercises on the user object to include this new workout in all contained exercises
			var ownedExerciseWorkout = exerciseIdToExercise[exerciseId].Workouts
				.First(x => x.WorkoutId == newWorkout.WorkoutId);
			ownedExerciseWorkout.WorkoutName = newWorkout.WorkoutName;
		}
	}

	public static void UpdateOwnedExercisesOnEdit(User user, Routine newRoutine, Workout workout)
	{
		var updateDefaultWeight = user.UserPreferences.UpdateDefaultWeightOnSave;
		var newExerciseIds = new HashSet<string>();
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

					newExerciseIds.Add(exerciseId);
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

		// find the exercises that are no longer being used in this workout
		var deletedExercises = oldExerciseIds.Where(x => !newExerciseIds.Contains(x)).ToList();

		foreach (var exerciseId in newExerciseIds)
		{
			var ownedExercise = exerciseIdToExercise[exerciseId];
			ownedExercise.Workouts.Add(new OwnedExerciseWorkout
			{
				WorkoutId = workout.WorkoutId,
				WorkoutName = workout.WorkoutName
			});
		}

		foreach (var exerciseId in deletedExercises)
		{
			var ownedExercise = exerciseIdToExercise[exerciseId];
			var ownedExerciseWorkout = ownedExercise.Workouts.First(x => x.WorkoutId == workout.WorkoutId);
			ownedExercise.Workouts.Remove(ownedExerciseWorkout);
		}
	}

	public static void VerifyCurrentDayAndWeek(Workout editedWorkout)
	{
		// make sure that the current week according to the frontend is actually valid
		var currentDay = editedWorkout.CurrentDay;
		var currentWeek = editedWorkout.CurrentWeek;
		if (currentWeek < 0 && currentWeek >= editedWorkout.Routine.Weeks.Count)
		{
			// frontend incorrectly set the current week, so just set both to 0
			editedWorkout.CurrentWeek = 0;
			editedWorkout.CurrentDay = 0;
			return;
		}

		if (currentDay < 0 && currentDay >= editedWorkout.Routine.Weeks[currentWeek].Days.Count)
		{
			// frontend incorrectly set the current day, so just set it to 0
			editedWorkout.CurrentWeek = 0;
			editedWorkout.CurrentDay = 0;
		}
	}

	/// <summary>
	/// Reset each exercise in the workout to be not completed, and update statistics and default weights where necessary.
	/// </summary>
	/// <param name="workout"></param>
	/// <param name="workoutMeta"></param>
	/// <param name="user"></param>
	public static void RestartWorkout(Workout workout, WorkoutMeta workoutMeta, User user)
	{
		var exerciseIdToExercise = user.Exercises.ToDictionary(x => x.Id, x => x);
		foreach (var week in workout.Routine.Weeks)
		{
			foreach (var day in week.Days)
			{
				foreach (var routineExercise in day.Exercises)
				{
					if (routineExercise.Completed)
					{
						workoutMeta.AverageExercisesCompleted =
							IncreaseAverage(workoutMeta.AverageExercisesCompleted, workoutMeta.TotalExercisesSum, 1);
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
						workoutMeta.AverageExercisesCompleted =
							IncreaseAverage(workoutMeta.AverageExercisesCompleted, workoutMeta.TotalExercisesSum, 0);
					}

					workoutMeta.TotalExercisesSum += 1;
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