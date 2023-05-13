namespace LiteWeightApi.Api.SharedWorkouts.Responses;

public class SharedDayResponse
{
	/// <summary>
	/// Arbitrary tag of the day.
	/// </summary>
	/// <example>Cardio and Legs Day</example>
	public string Tag { get; set; }

	/// <summary>
	/// List of exercises for the given day.
	/// </summary>
	public IList<SharedExerciseResponse> Exercises { get; set; } = new List<SharedExerciseResponse>();
}