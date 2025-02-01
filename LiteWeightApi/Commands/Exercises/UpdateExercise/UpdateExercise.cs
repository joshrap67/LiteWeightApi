namespace LiteWeightAPI.Commands.Exercises.UpdateExercise;

public class UpdateExercise : ICommand<bool>
{
	public required string UserId { get; set; }
	
	public required string ExerciseId { get; set; }

	public required string Name { get; set; }

	public double DefaultWeight { get; set; }

	public int DefaultSets { get; set; }

	public int DefaultReps { get; set; }

	public IList<string> Focuses { get; set; } = new List<string>();

	public string? DefaultDetails { get; set; }

	public string? VideoUrl { get; set; }
}