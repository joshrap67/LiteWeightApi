using LiteWeightAPI.Api.CurrentUser.Responses;
using LiteWeightAPI.Api.Exercises.Responses;

namespace LiteWeightAPI.Api.SharedWorkouts.Responses;

public class AcceptSharedWorkoutResponse
{
	/// <summary>
	/// Information of the workout that was created as a result of accepting the workout.
	/// </summary>
	public WorkoutInfoResponse NewWorkoutInfo { get; set; }

	/// <summary>
	/// New exercises that were created as a result of accepting the workout.
	/// </summary>
	public IEnumerable<OwnedExerciseResponse> NewExercises { get; set; }
}