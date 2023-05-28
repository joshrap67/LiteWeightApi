using LiteWeightAPI.Api.Exercises.Responses;
using LiteWeightAPI.Api.Self.Responses;

namespace LiteWeightAPI.Api.SharedWorkouts.Responses;

public class AcceptSharedWorkoutResponse
{
	/// <summary>
	/// Information of the workout that was created as a result of accepting the workout.
	/// </summary>
	public WorkoutInfoResponse NewWorkoutInfo { get; set; }

	/// <summary>
	/// New total list of exercises owned by the user as a result of accepting this workout. New exercises may have been added if the user did not already have them.
	/// </summary>
	public IEnumerable<OwnedExerciseResponse> UserExercises { get; set; }
}