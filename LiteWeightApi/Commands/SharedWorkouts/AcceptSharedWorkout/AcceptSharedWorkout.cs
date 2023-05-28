using LiteWeightAPI.Api.SharedWorkouts.Responses;

namespace LiteWeightAPI.Commands.SharedWorkouts.AcceptSharedWorkout;

public class AcceptSharedWorkout : ICommand<AcceptSharedWorkoutResponse>
{
	public string UserId { get; set; }
	public string SharedWorkoutId { get; set; }
	public string NewName { get; set; }
}