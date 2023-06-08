using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightApiTests.Domain.Workouts;

public class RoutineTests : BaseTest
{
	[Fact]
	public void Should_Be_Created_From_Shared_Routine()
	{
		var exerciseNameToId = new Dictionary<string, string>
		{
			{ "A", Fixture.Create<string>() },
			{ "B", Fixture.Create<string>() },
			{ "C", Fixture.Create<string>() },
			{ "D", Fixture.Create<string>() },
			{ "E", Fixture.Create<string>() },
		};
		var sharedRoutine = Fixture.Build<SharedRoutine>()
			.With(x => x.Weeks, new List<SharedWeek>
			{
				Fixture.Build<SharedWeek>().With(x => x.Days, new List<SharedDay>
				{
					Fixture.Build<SharedDay>().With(x => x.Exercises, new List<SharedExercise>
					{
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "A").Create(),
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "B").Create(),
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "C").Create()
					}).Create(),
					Fixture.Build<SharedDay>().With(x => x.Exercises, new List<SharedExercise>
					{
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "E").Create(),
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "B").Create()
					}).Create(),
					Fixture.Build<SharedDay>().With(x => x.Exercises, new List<SharedExercise>
					{
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "E").Create()
					}).Create()
				}).Create(),
				Fixture.Build<SharedWeek>().With(x => x.Days, new List<SharedDay>
				{
					Fixture.Build<SharedDay>().With(x => x.Exercises, new List<SharedExercise>
					{
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "A").Create(),
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "B").Create(),
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "C").Create(),
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "D").Create(),
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "E").Create(),
					}).Create(),
					Fixture.Build<SharedDay>().With(x => x.Exercises, new List<SharedExercise>
					{
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "A").Create(),
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "B").Create()
					}).Create(),
					Fixture.Build<SharedDay>().With(x => x.Exercises, new List<SharedExercise>
					{
						Fixture.Build<SharedExercise>().With(x => x.ExerciseName, "D").Create()
					}).Create()
				}).Create()
			}).Create();

		var routine = new Routine(sharedRoutine, exerciseNameToId);
		Assert.Equal(sharedRoutine.Weeks.Sum(x => x.Days.Count), routine.TotalNumberOfDays);

		for (var i = 0; i < routine.Weeks.Count; i++)
		{
			for (var j = 0; j < routine.Weeks[i].Days.Count; j++)
			{
				var sharedRoutineDayTag = sharedRoutine.Weeks[i].Days[j].Tag;
				var routineDayTag = routine.Weeks[i].Days[j].Tag;
				Assert.Equal(sharedRoutineDayTag, routineDayTag);

				for (var k = 0; k < routine.Weeks[i].Days[j].Exercises.Count; k++)
				{
					var sharedRoutineExerciseName = sharedRoutine.Weeks[i].Days[j].Exercises[k].ExerciseName;
					var exerciseId = routine.Weeks[i].Days[j].Exercises[k].ExerciseId;
					Assert.Equal(exerciseNameToId[sharedRoutineExerciseName], exerciseId);
				}
			}
		}
	}
}