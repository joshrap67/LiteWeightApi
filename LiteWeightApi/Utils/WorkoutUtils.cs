using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightAPI.Utils;

public static class WorkoutUtils
{
	public static void UpdateOwnedExercisesOnCreation(User user, Workout newWorkout, bool updateWeight)
	{
		var updateDefaultWeight = user.Preferences.UpdateDefaultWeightOnSave && updateWeight;
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
}