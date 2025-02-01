using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Commands.Workouts.GetWorkout;

public class GetWorkout : ICommand<WorkoutResponse>
{
	public required string UserId { get; set; }
	public required string WorkoutId { get; set; }
}