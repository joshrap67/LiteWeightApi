using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Commands.Workouts.UpdateRoutine;

public class UpdateRoutine : ICommand<UserAndWorkoutResponse>
{
	public required string UserId { get; set; }
	public required string WorkoutId { get; set; }
	public required SetRoutine Routine { get; set; }
}