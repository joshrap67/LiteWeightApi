using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightAPI.Domain.SharedWorkouts;

public class SharedExercise
{
	public SharedExercise(RoutineExercise exercise, string exerciseName)
	{
		ExerciseName = exerciseName;
		Weight = exercise.Weight;
		Sets = exercise.Sets;
		Reps = exercise.Reps;
		Details = exercise.Details;
	}

	public string ExerciseName { get; set; }
	public double Weight { get; set; }
	public int Sets { get; set; }
	public int Reps { get; set; }
	public string Details { get; set; }
}