using AutoMapper;
using LiteWeightAPI.Api.Exercises;
using LiteWeightAPI.Commands.Workouts;
using LiteWeightAPI.Commands.Workouts.CreateWorkout;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Imports;
using NodaTime;

namespace LiteWeightApiTests.Commands.Workouts;

public class CreateWorkoutTests
{
	private readonly CreateWorkoutHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public CreateWorkoutTests()
	{
		var configuration = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(ExercisesController)); });
		var mapper = new Mapper(configuration);

		_mockRepository = new Mock<IRepository>();
		var clock = new Mock<IClock>();
		_handler = new CreateWorkoutHandler(_mockRepository.Object, clock.Object, mapper);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task Should_Create(bool setAsCurrentWorkout)
	{
		var command = _fixture.Build<CreateWorkout>()
			.With(x => x.SetAsCurrentWorkout, setAsCurrentWorkout)
			.Create();
		var workouts = Enumerable.Range(0, Globals.MaxFreeWorkouts / 2)
			.Select(_ => _fixture.Create<WorkoutInfo>())
			.ToList();
		var exercisesOfWorkout = command.Routine.Weeks
			.SelectMany(x => x.Days)
			.SelectMany(x => x.Exercises)
			.Select(x => x.ExerciseId)
			.ToList();
		var ownedExercises = Enumerable.Range(0, 10)
			.Select(_ => _fixture.Create<OwnedExercise>())
			.ToList();
		ownedExercises.AddRange(exercisesOfWorkout.Select(exerciseId =>
			_fixture.Build<OwnedExercise>().With(x => x.Id, exerciseId).Create()));

		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.With(x => x.Exercises, ownedExercises)
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		var response = await _handler.HandleAsync(command);
		// all exercises of workout should have this workout after the copy
		Assert.True(user.Exercises.Where(x => exercisesOfWorkout.Contains(x.Id))
			.All(x => x.Workouts.Any(y => y.WorkoutId == response.Workout.Id)));
		// exercises not apart of the workout should not have changed
		Assert.True(user.Exercises.Where(x => !exercisesOfWorkout.Contains(x.Id))
			.All(x => x.Workouts.All(y => y.WorkoutId != response.Workout.Id)));
		Assert.Contains(user.Workouts, x => x.WorkoutId == response.Workout.Id);
		if (setAsCurrentWorkout)
		{
			Assert.Equal(user.CurrentWorkoutId, response.Workout.Id);
		}
	}

	[Theory]
	[MemberData(nameof(InvalidRoutineCases))]
	public async Task Should_Throw_Exception_Invalid_Routine(SetRoutine setRoutine)
	{
		var command = _fixture.Build<CreateWorkout>()
			.With(x => x.Routine, setRoutine)
			.Create();

		var workouts = Enumerable.Range(0, Globals.MaxWorkouts / 2)
			.Select(_ => _fixture.Create<WorkoutInfo>())
			.ToList();

		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<InvalidRoutineException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Workout_Name_Duplicate()
	{
		var command = _fixture.Create<CreateWorkout>();
		var workouts = Enumerable.Range(0, Globals.MaxWorkouts / 2)
			.Select(_ => _fixture.Build<WorkoutInfo>().With(y => y.WorkoutName, command.WorkoutName).Create())
			.ToList();
		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<AlreadyExistsException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Max_Workouts()
	{
		var command = _fixture.Create<CreateWorkout>();
		var workouts = Enumerable.Range(0, Globals.MaxWorkouts + 1)
			.Select(_ => _fixture.Build<WorkoutInfo>().Create())
			.ToList();
		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<MaxLimitException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Max_Free_Workouts()
	{
		var command = _fixture.Create<CreateWorkout>();
		var workouts = Enumerable.Range(0, Globals.MaxFreeWorkouts + 1)
			.Select(_ => _fixture.Build<WorkoutInfo>().Create())
			.ToList();
		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.With(x => x.PremiumToken, (string)null)
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<MaxLimitException>(() => _handler.HandleAsync(command));
	}

	public static IEnumerable<object[]> InvalidRoutineCases
	{
		get
		{
			var fixture = new Fixture();
			var maxDays = Enumerable.Range(0, Globals.MaxDaysRoutine + 1)
				.Select(_ => fixture.Create<SetRoutineDay>())
				.ToList();
			var maxExercises = Enumerable.Range(0, Globals.MaxExercises + 1)
				.Select(_ => fixture.Create<SetRoutineExercise>())
				.ToList();
			var maxWeeks = Enumerable.Range(0, Globals.MaxWeeksRoutine + 1)
				.Select(_ => fixture.Create<SetRoutineWeek>())
				.ToList();
			yield return new object[]
			{
				new SetRoutine
				{
					Weeks = maxWeeks
				}
			};
			yield return new object[]
			{
				new SetRoutine
				{
					Weeks = new List<SetRoutineWeek>
					{
						new()
						{
							Days = maxDays
						}
					}
				}
			};
			yield return new object[]
			{
				new SetRoutine
				{
					Weeks = new List<SetRoutineWeek>
					{
						new()
						{
							Days = new List<SetRoutineDay> { new() { Exercises = maxExercises } }
						}
					}
				}
			};
			yield return new object[]
			{
				new SetRoutine
				{
					Weeks = new List<SetRoutineWeek>
					{
						new()
						{
							Days = new List<SetRoutineDay>
							{
								new()
								{
									Exercises = new List<SetRoutineExercise>(),
									Tag = string.Join("", fixture.CreateMany<char>(Globals.MaxDayTagLength + 1))
								}
							}
						}
					}
				}
			};
		}
	}
}