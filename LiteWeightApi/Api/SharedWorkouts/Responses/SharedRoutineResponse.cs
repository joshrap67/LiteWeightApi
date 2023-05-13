namespace LiteWeightApi.Api.SharedWorkouts.Responses;

public class SharedRoutineResponse
{
	/// <summary>
	/// List of weeks in the routine.
	/// </summary>
	public IList<SharedWeekResponse> Weeks { get; set; } = new List<SharedWeekResponse>();
}