using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Commands.Workouts.UpdateRoutine;

public class UpdateRoutine : ICommand<UserAndWorkoutResponse>
{
	public string UserId { get; set; }
	public string WorkoutId { get; set; }
	public SetRoutine Routine { get; set; }
}