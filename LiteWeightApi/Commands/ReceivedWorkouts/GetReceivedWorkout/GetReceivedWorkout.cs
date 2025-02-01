using LiteWeightAPI.Api.ReceivedWorkouts.Responses;

namespace LiteWeightAPI.Commands.ReceivedWorkouts.GetReceivedWorkout;

public class GetReceivedWorkout : ICommand<ReceivedWorkoutResponse>
{
	public required string UserId { get; set; }
	public required string ReceivedWorkoutId { get; set; }
}