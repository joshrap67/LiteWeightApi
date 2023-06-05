using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Commands.Workouts.CopyWorkout;

public class CopyWorkout : ICommand<UserAndWorkoutResponse>
{
	public string UserId { get; set; }
	public string WorkoutId { get; set; }
	public string Name { get; set; }
}