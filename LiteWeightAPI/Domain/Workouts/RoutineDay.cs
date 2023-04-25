namespace LiteWeightAPI.Domain.Workouts;

public class RoutineDay
{
	public IList<RoutineExercise> Exercises { get; set; } = new List<RoutineExercise>();
	public string Tag { get; set; }

	public RoutineDay Clone()
	{
		var clonedDay = new RoutineDay
		{
			Tag = Tag
		};
		foreach (var exercise in Exercises)
		{
			clonedDay.Exercises.Add(exercise.Clone());
		}

		return clonedDay;
	}

	public void AppendExercise(RoutineExercise exercise)
	{
		Exercises.Add(exercise);
	}
}