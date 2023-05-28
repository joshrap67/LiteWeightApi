using LiteWeightAPI.Api.Workouts.Responses;

namespace LiteWeightAPI.Commands.Workouts.GetWorkout;

public class GetWorkout : ICommand<WorkoutResponse>
{
	public string UserId { get; set; }
	public string WorkoutId { get; set; }
}