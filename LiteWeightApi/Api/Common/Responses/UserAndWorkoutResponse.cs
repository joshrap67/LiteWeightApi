using LiteWeightApi.Api.CurrentUser.Responses;
using LiteWeightApi.Api.Workouts.Responses;

namespace LiteWeightApi.Api.Common.Responses;

public class UserAndWorkoutResponse
{
	/// <summary>
	/// Updated user.
	/// </summary>
	public UserResponse User { get; set; }

	/// <summary>
	/// Updated workout.
	/// </summary>
	public WorkoutResponse Workout { get; set; }
}