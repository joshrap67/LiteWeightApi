using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Commands.Workouts.CreateWorkout;

public class CreateWorkout : ICommand<UserAndWorkoutResponse>
{
	public string UserId { get; set; }
	public string WorkoutName { get; set; }
	public SetRoutine Routine { get; set; }
	public bool SetAsCurrentWorkout { get; set; }
}