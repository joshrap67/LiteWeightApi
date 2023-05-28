namespace LiteWeightAPI.Commands.Exercises.UpdateExercise;

public class UpdateExercise : ICommand<bool>
{
	public string UserId { get; set; }
	
	public string ExerciseId { get; set; }

	public string Name { get; set; }

	public double DefaultWeight { get; set; }

	public int DefaultSets { get; set; }

	public int DefaultReps { get; set; }

	public IList<string> Focuses { get; set; } = new List<string>();

	public string DefaultDetails { get; set; }

	public string VideoUrl { get; set; }
}