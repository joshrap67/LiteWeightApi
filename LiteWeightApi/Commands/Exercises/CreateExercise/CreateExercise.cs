using LiteWeightAPI.Api.Exercises.Responses;

namespace LiteWeightAPI.Commands.Exercises.CreateExercise;

public class CreateExercise : ICommand<OwnedExerciseResponse>
{
	public string UserId { get; set; }

	public string Name { get; set; }

	public double DefaultWeight { get; set; }

	public int DefaultSets { get; set; }

	public int DefaultReps { get; set; }

	public IList<string> Focuses { get; set; } = new List<string>();

	public string DefaultDetails { get; set; }

	public string VideoUrl { get; set; }
}