namespace LiteWeightAPI.Domain.SharedWorkouts;

public class SharedWeek
{
	public void AppendDay(SharedDay sharedDay)
	{
		Days.Add(sharedDay);
	}

	public IList<SharedDay> Days { get; set; } = new List<SharedDay>();
}