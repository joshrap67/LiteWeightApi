namespace LiteWeightAPI.Domain.Workouts;

public class Workout
{
	public string WorkoutId { get; set; }
	public string WorkoutName { get; set; }
	public string CreationDate { get; set; }
	public string Creator { get; set; }
	public Routine Routine { get; set; }
	public int CurrentDay { get; set; }
	public int CurrentWeek { get; set; }
}