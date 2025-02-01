using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Commands.Workouts.CreateWorkout;

public class CreateWorkout : ICommand<UserAndWorkoutResponse>
{
	public required string UserId { get; set; }
	public required string Name { get; set; }
	public required SetRoutine Routine { get; set; }
	public bool SetAsCurrentWorkout { get; set; }
}