namespace LiteWeightAPI.Commands.Exercises.DeleteExercise;

public class DeleteExercise : ICommand<bool>
{
	public required string UserId { get; set; }

	public required string ExerciseId { get; set; }
}