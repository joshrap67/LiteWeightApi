namespace LiteWeightApi.Api.SharedWorkouts.Responses;

public class SharedWeekResponse
{
	/// <summary>
	/// List of days in the routine.
	/// </summary>
	public IList<SharedDayResponse> Days { get; set; } = new List<SharedDayResponse>();
}