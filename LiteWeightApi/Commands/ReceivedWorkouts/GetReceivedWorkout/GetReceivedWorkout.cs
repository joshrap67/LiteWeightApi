using LiteWeightAPI.Api.ReceivedWorkouts.Responses;

namespace LiteWeightAPI.Commands.ReceivedWorkouts.GetReceivedWorkout;

public class GetReceivedWorkout : ICommand<ReceivedWorkoutResponse>
{
	public string UserId { get; set; }
	public string ReceivedWorkoutId { get; set; }
}