using LiteWeightAPI.Api.SharedWorkouts.Responses;

namespace LiteWeightAPI.Commands.SharedWorkouts.GetSharedWorkout;

public class GetSharedWorkout : ICommand<SharedWorkoutResponse>
{
	public string UserId { get; set; }
	public string SharedWorkoutId { get; set; }
}