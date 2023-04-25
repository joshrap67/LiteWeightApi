namespace LiteWeightAPI.Domain.Users;

public class OwnedExercise
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public string ExerciseName { get; set; }
	public double DefaultWeight { get; set; } // stored in lbs
	public int DefaultSets { get; set; } = 3;
	public int DefaultReps { get; set; } = 15;
	public string DefaultDetails { get; set; } = "";
	public string VideoUrl { get; set; } = "";
	public IList<string> Focuses { get; set; } = new List<string>();
	public IList<OwnedExerciseWorkout> Workouts { get; set; } = new List<OwnedExerciseWorkout>();

	public void Update(string exerciseName, double defaultWeight, int defaultSets, int defaultReps,
		string defaultDetails, string videoUrl)
	{
		ExerciseName = exerciseName;
		DefaultWeight = defaultWeight;
		DefaultSets = defaultSets;
		DefaultReps = defaultReps;
		DefaultDetails = defaultDetails;
		VideoUrl = videoUrl;
	}
}