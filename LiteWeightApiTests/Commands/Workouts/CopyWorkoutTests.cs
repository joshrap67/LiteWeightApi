using AutoMapper;
using LiteWeightAPI.Api.Exercises;
using LiteWeightAPI.Commands.Workouts.CopyWorkout;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;
using NodaTime;

namespace LiteWeightApiTests.Commands.Workouts;

public class CopyWorkoutTests
{
	private readonly CopyWorkoutHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public CopyWorkoutTests()
	{
		var configuration = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(ExercisesController)); });
		var mapper = new Mapper(configuration);

		_mockRepository = new Mock<IRepository>();
		var clock = new Mock<IClock>();
		_handler = new CopyWorkoutHandler(_mockRepository.Object, clock.Object, mapper);
	}

	[Fact]
	public async Task Should_Copy()
	{
		var command = _fixture.Create<CopyWorkout>();
		var workouts = Enumerable.Range(0, Globals.MaxFreeWorkouts / 2)
			.Select(_ => _fixture.Create<WorkoutInfo>())
			.ToList();
		var workout = _fixture.Build<Workout>()
			.With(x => x.CreatorId, command.UserId)
			.Create();
		var exercisesOfWorkout = workout.Routine.Weeks
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
			.Setup(x => x.GetWorkout(It.Is<string>(y => y == command.WorkoutId)))
			.ReturnsAsync(workout);

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
	}

	[Fact]
	public async Task Should_Throw_Exception_Workout_Name_Duplicate()
	{
		var command = _fixture.Create<CopyWorkout>();
		var workouts = Enumerable.Range(0, Globals.MaxWorkouts / 2)
			.Select(_ => _fixture.Build<WorkoutInfo>().With(y => y.WorkoutName, command.Name).Create())
			.ToList();
		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.Create();

		var workout = _fixture.Build<Workout>()
			.With(x => x.CreatorId, user.Id)
			.Create();

		_mockRepository
			.Setup(x => x.GetWorkout(It.Is<string>(y => y == command.WorkoutId)))
			.ReturnsAsync(workout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<AlreadyExistsException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Max_Workouts()
	{
		var command = _fixture.Create<CopyWorkout>();
		var workouts = Enumerable.Range(0, Globals.MaxWorkouts + 1)
			.Select(_ => _fixture.Build<WorkoutInfo>().Create())
			.ToList();
		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.Create();
		var workout = _fixture.Build<Workout>()
			.With(x => x.CreatorId, user.Id)
			.Create();

		_mockRepository
			.Setup(x => x.GetWorkout(It.Is<string>(y => y == command.WorkoutId)))
			.ReturnsAsync(workout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<MaxLimitException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Max_Free_Workouts()
	{
		var command = _fixture.Create<CopyWorkout>();
		var workouts = Enumerable.Range(0, Globals.MaxFreeWorkouts + 1)
			.Select(_ => _fixture.Build<WorkoutInfo>().Create())
			.ToList();
		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.With(x => x.PremiumToken, (string)null)
			.Create();
		var workout = _fixture.Build<Workout>()
			.With(x => x.CreatorId, user.Id)
			.Create();

		_mockRepository
			.Setup(x => x.GetWorkout(It.Is<string>(y => y == command.WorkoutId)))
			.ReturnsAsync(workout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<MaxLimitException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Missing_Permissions_Workout()
	{
		var command = _fixture.Create<CopyWorkout>();
		var user = _fixture.Create<User>();

		_mockRepository
			.Setup(x => x.GetWorkout(It.Is<string>(y => y == command.WorkoutId)))
			.ReturnsAsync(_fixture.Create<Workout>());

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<ForbiddenException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Workout_Not_Found()
	{
		var command = _fixture.Create<CopyWorkout>();

		_mockRepository
			.Setup(x => x.GetWorkout(It.Is<string>(y => y == command.WorkoutId)))
			.ReturnsAsync((Workout)null);

		await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.HandleAsync(command));
	}
}