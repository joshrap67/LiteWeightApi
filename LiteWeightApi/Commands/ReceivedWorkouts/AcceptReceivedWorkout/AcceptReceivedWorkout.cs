using LiteWeightAPI.Api.ReceivedWorkouts.Responses;

namespace LiteWeightAPI.Commands.ReceivedWorkouts.AcceptReceivedWorkout;

public class AcceptReceivedWorkout : ICommand<AcceptReceivedWorkoutResponse>
{
	public required string UserId { get; set; }
	public required string ReceivedWorkoutId { get; set; }
	public string? NewName { get; set; }
}