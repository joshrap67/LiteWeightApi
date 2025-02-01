using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Commands.Workouts.CopyWorkout;

public class CopyWorkout : ICommand<UserAndWorkoutResponse>
{
	public required string UserId { get; set; }
	public required string WorkoutId { get; set; }
	public required string Name { get; set; }
}