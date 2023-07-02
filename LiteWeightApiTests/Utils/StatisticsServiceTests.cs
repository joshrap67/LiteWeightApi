using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Services;

namespace LiteWeightApiTests.Utils;

public class StatisticsServiceTests : BaseTest
{
	private readonly StatisticsService _service;

	public StatisticsServiceTests()
	{
		_service = new StatisticsService();
	}

	private Routine GetRoutine()
	{
		return new Routine
		{
			Weeks = new List<RoutineWeek>
			{
				new()
				{
					Days =
					{
						new RoutineDay
						{
							Exercises = new List<RoutineExercise>
							{
								Fixture.Build<RoutineExercise>().With(x => x.ExerciseId, "A").Create(),
								Fixture.Build<RoutineExercise>().With(x => x.ExerciseId, "B").Create(),
								Fixture.Build<RoutineExercise>().With(x => x.ExerciseId, "C").Create()
							}
						},
						new RoutineDay
						{
							Exercises = new List<RoutineExercise>
							{
								Fixture.Build<RoutineExercise>().With(x => x.ExerciseId, "A").Create(),
								Fixture.Build<RoutineExercise>().With(x => x.ExerciseId, "B").Create(),
								Fixture.Build<RoutineExercise>().With(x => x.ExerciseId, "B").Create(),
								Fixture.Build<RoutineExercise>().With(x => x.ExerciseId, "D").Create()
							}
						}
					}
				},
				new()
				{
					Days =
					{
						new RoutineDay
						{
							Exercises = new List<RoutineExercise>
							{
								Fixture.Build<RoutineExercise>().With(x => x.ExerciseId, "A").Create(),
								Fixture.Build<RoutineExercise>().With(x => x.ExerciseId, "D").Create()
							}
						},
						new RoutineDay
						{
							Exercises = new List<RoutineExercise>
							{
								Fixture.Build<RoutineExercise>().With(x => x.ExerciseId, "B").Create(),
								Fixture.Build<RoutineExercise>().With(x => x.ExerciseId, "E").Create()
							}
						}
					}
				}
			}
		};
	}

	[Theory]
	[MemberData(nameof(FocusTestCases))]
	public void FindMostFrequentFocus(Dictionary<string, OwnedExercise> exerciseIdToExercise, string expectedValue)
	{
		var mostFrequentFocus = _service.FindMostFrequentFocus(exerciseIdToExercise, GetRoutine());
		Assert.Equal(expectedValue, mostFrequentFocus);
	}

	public static IEnumerable<object[]> FocusTestCases
	{
		get
		{
			var fixture = new Fixture();
			yield return new object[]
			{
				new Dictionary<string, OwnedExercise>
				{
					{
						"A",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Back", "Biceps" })
							.Create()
					},
					{
						"B",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Cardio", "Legs" })
							.Create()
					},
					{
						"C",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Back", "Shoulders" })
							.Create()
					},
					{
						"D",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Back", "Triceps" })
							.Create()
					},
					{
						"E",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Biceps", "Forearms" })
							.Create()
					}
				},
				"Back"
			};
			yield return new object[]
			{
				new Dictionary<string, OwnedExercise>
				{
					{
						"A",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Legs", "Cardio" })
							.Create()
					},
					{
						"B",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Cardio", "Legs" })
							.Create()
					},
					{
						"C",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Back", "Shoulders" })
							.Create()
					},
					{
						"D",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Back", "Triceps" })
							.Create()
					},
					{
						"E",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Biceps", "Forearms" })
							.Create()
					}
				},
				"Legs,Cardio"
			};
			yield return new object[]
			{
				new Dictionary<string, OwnedExercise>
				{
					{
						"A",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Back", "Biceps" })
							.Create()
					},
					{
						"B",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Biceps", "Legs" })
							.Create()
					},
					{
						"C",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Biceps", "Shoulders" })
							.Create()
					},
					{
						"D",
						fixture.Build<OwnedExercise>().With(x => x.Focuses,
								new List<string> { "Strength Training", "Triceps" })
							.Create()
					},
					{
						"E",
						fixture.Build<OwnedExercise>().With(x => x.Focuses, new List<string> { "Biceps", "Forearms" })
							.Create()
					}
				},
				"Biceps"
			};
		}
	}
}