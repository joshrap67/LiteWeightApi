using LiteWeightAPI.Api.ReceivedWorkouts.Responses;

namespace LiteWeightAPI.Commands.ReceivedWorkouts.AcceptReceivedWorkout;

public class AcceptReceivedWorkout : ICommand<AcceptReceivedWorkoutResponse>
{
	public string UserId { get; set; }
	public string ReceivedWorkoutId { get; set; }
	public string NewName { get; set; }
}