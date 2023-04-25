using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightAPI.Domain.SharedWorkouts;

public class SharedWorkout
{
	public string SharedWorkoutId { get; set; }
	public string WorkoutName { get; set; }
	public string Creator { get; set; }
	public SharedRoutine Routine { get; set; }
	public IList<SharedWorkoutExercise> Exercises { get; set; }

	public SharedWorkout(Workout workout, string sharedWorkoutId, User user)
	{
		SharedWorkoutId = sharedWorkoutId;
		WorkoutName = workout.WorkoutName;
		Creator = workout.Creator;
		Routine = new SharedRoutine(workout.Routine, user.Exercises);

		// preserve the focuses and video url of the exercises since recipient user might not have the same exercises
		Exercises = new List<SharedWorkoutExercise>();
		var exerciseIdToExercise = user.Exercises.ToDictionary(x => x.Id, x => x);
		var exercisesOfWorkout = workout.Routine.Weeks
			.SelectMany(x => x.Days)
			.SelectMany(x => x.Exercises)
			.DistinctBy(x => x.ExerciseId).ToList();
		foreach (var routineExercise in exercisesOfWorkout)
		{
			var ownedExercise = exerciseIdToExercise[routineExercise.ExerciseId];
			var sharedWorkoutExercise = new SharedWorkoutExercise(ownedExercise, ownedExercise.ExerciseName);
			Exercises.Add(sharedWorkoutExercise);
		}
	}
}