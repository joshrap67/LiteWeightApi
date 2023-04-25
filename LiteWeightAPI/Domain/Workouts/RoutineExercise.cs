namespace LiteWeightAPI.Domain.Workouts;

public class RoutineExercise
{
	public bool Completed { get; set; }
	public string ExerciseId { get; set; }
	public double Weight { get; set; }
	public int Sets { get; set; }
	public int Reps { get; set; }
	public string Details { get; set; }

	public RoutineExercise Clone()
	{
		var copy = new RoutineExercise
		{
			Completed = Completed,
			Details = Details,
			Reps = Reps,
			Sets = Sets,
			Weight = Weight,
			ExerciseId = ExerciseId
		};
		return copy;
	}
}