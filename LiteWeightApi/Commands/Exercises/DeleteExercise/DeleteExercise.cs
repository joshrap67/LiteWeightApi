namespace LiteWeightAPI.Commands.Exercises.DeleteExercise;

public class DeleteExercise : ICommand<bool>
{
	public string UserId { get; set; }

	public string ExerciseId { get; set; }
}