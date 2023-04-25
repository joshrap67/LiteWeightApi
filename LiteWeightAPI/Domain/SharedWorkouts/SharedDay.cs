namespace LiteWeightAPI.Domain.SharedWorkouts;

public class SharedDay
{
	public void AppendExercise(SharedExercise sharedExercise)
	{
		Exercises.Add(sharedExercise);
	}

	public IList<SharedExercise> Exercises { get; set; } = new List<SharedExercise>();
	public string Tag { get; set; }
}