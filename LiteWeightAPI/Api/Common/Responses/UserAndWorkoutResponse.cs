using LiteWeightAPI.Api.Users.Responses;
using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Api.Common.Responses;

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