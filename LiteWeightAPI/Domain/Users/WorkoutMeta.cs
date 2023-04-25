namespace LiteWeightAPI.Domain.Users;

public class WorkoutMeta
{
	public string WorkoutId { get; set; }
	public string WorkoutName { get; set; }
	public string DateLast { get; set; }
	public int TimesCompleted { get; set; }
	public double AverageExercisesCompleted { get; set; }
	public int TotalExercisesSum { get; set; }
}