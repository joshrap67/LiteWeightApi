namespace LiteWeightAPI.Domain.Workouts;

public class RoutineWeek
{
	public IList<RoutineDay> Days { get; set; } = new List<RoutineDay>();

	public void AppendDay(RoutineDay routineDay)
	{
		Days.Add(routineDay);
	}
}