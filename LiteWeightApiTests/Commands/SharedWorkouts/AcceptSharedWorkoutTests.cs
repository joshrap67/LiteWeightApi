using AutoMapper;
using LiteWeightAPI.Api.Exercises;
using LiteWeightAPI.Commands.SharedWorkouts.AcceptSharedWorkout;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;
using LiteWeightApiTests.TestHelpers;
using NodaTime;

namespace LiteWeightApiTests.Commands.SharedWorkouts;

public class AcceptSharedWorkoutTests
{
	private readonly AcceptSharedWorkoutHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public AcceptSharedWorkoutTests()
	{
		var configuration = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(ExercisesController)); });
		var mapper = new Mapper(configuration);

		_mockRepository = new Mock<IRepository>();
		var clock = new Mock<IClock>();
		_handler = new AcceptSharedWorkoutHandler(_mockRepository.Object, clock.Object, mapper);
	}

	[Fact]
	public async Task Should_Accept_Workout()
	{
		// todo separate unit tests for constructors of shared workout
		var command = _fixture.Create<AcceptSharedWorkout>();

		var sharedWorkout = SharedWorkoutHelper.GetSharedWorkout(command.UserId);

		var workouts = Enumerable.Range(0, Globals.MaxWorkouts / 2)
			.Select(_ => _fixture.Create<WorkoutInfo>())
			.ToList();

		var originalExerciseCount = (Globals.MaxExercises - sharedWorkout.DistinctExercises.Count) / 2;
		var ownedExercises = Enumerable.Range(0, originalExerciseCount)
			.Select(_ => _fixture.Create<OwnedExercise>())
			.ToList();

		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.With(x => x.Exercises, ownedExercises)
			.Create();

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync(sharedWorkout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		var response = await _handler.HandleAsync(command);
		Assert.Equal(originalExerciseCount + sharedWorkout.DistinctExercises.Count, user.Exercises.Count);
		Assert.Equal(originalExerciseCount + sharedWorkout.DistinctExercises.Count, response.UserExercises.Count());
		Assert.Equal(command.NewName, response.NewWorkoutInfo.WorkoutName);
		Assert.Contains(user.Workouts, x => x.WorkoutId == response.NewWorkoutInfo.WorkoutId);
	}

	[Fact]
	public async Task Should_Accept_Workout_Name_Not_Specified()
	{
		var command = _fixture.Build<AcceptSharedWorkout>().With(x => x.NewName, (string)null).Create();

		var sharedWorkout = SharedWorkoutHelper.GetSharedWorkout(command.UserId);

		var workouts = Enumerable.Range(0, Globals.MaxWorkouts / 2)
			.Select(_ => _fixture.Create<WorkoutInfo>())
			.ToList();

		var originalExerciseCount = (Globals.MaxExercises - sharedWorkout.DistinctExercises.Count) / 2;
		var ownedExercises = Enumerable.Range(0, originalExerciseCount)
			.Select(_ => _fixture.Create<OwnedExercise>())
			.ToList();

		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.With(x => x.Exercises, ownedExercises)
			.Create();

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync(sharedWorkout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		var response = await _handler.HandleAsync(command);
		Assert.Equal(originalExerciseCount + sharedWorkout.DistinctExercises.Count, user.Exercises.Count);
		Assert.Equal(originalExerciseCount + sharedWorkout.DistinctExercises.Count, response.UserExercises.Count());
		Assert.Equal(sharedWorkout.WorkoutName, response.NewWorkoutInfo.WorkoutName);
		Assert.Contains(user.Workouts, x => x.WorkoutId == response.NewWorkoutInfo.WorkoutId);
	}

	[Fact]
	public async Task Should_Throw_Exception_Workout_Does_Not_Exist()
	{
		var command = _fixture.Create<AcceptSharedWorkout>();

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync((SharedWorkout)null!);

		await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Missing_Permissions_Workout()
	{
		var command = _fixture.Create<AcceptSharedWorkout>();
		var user = _fixture.Create<User>();

		var sharedWorkout = SharedWorkoutHelper.GetSharedWorkout();

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync(sharedWorkout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<ForbiddenException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Max_Free_Workouts_Exceeded()
	{
		var command = _fixture.Create<AcceptSharedWorkout>();
		var workouts = Enumerable.Range(0, Globals.MaxFreeWorkouts)
			.Select(_ => _fixture.Create<WorkoutInfo>())
			.ToList();
		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.With(x => x.PremiumToken, (string)null)
			.Create();

		var sharedWorkout = SharedWorkoutHelper.GetSharedWorkout(command.UserId);

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync(sharedWorkout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<MaxLimitException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Max_Workouts_Exceeded()
	{
		var command = _fixture.Create<AcceptSharedWorkout>();
		var workouts = Enumerable.Range(0, Globals.MaxWorkouts)
			.Select(_ => _fixture.Create<WorkoutInfo>())
			.ToList();
		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.Create();

		var sharedWorkout = SharedWorkoutHelper.GetSharedWorkout(command.UserId);

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync(sharedWorkout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<MaxLimitException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Workout_Name_Duplicate_From_Command()
	{
		var command = _fixture.Create<AcceptSharedWorkout>();
		var workouts = Enumerable.Range(0, Globals.MaxWorkouts / 2)
			.Select(_ => _fixture.Build<WorkoutInfo>().With(y => y.WorkoutName, command.NewName).Create())
			.ToList();
		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.Create();

		var sharedWorkout = SharedWorkoutHelper.GetSharedWorkout(command.UserId);

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync(sharedWorkout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<AlreadyExistsException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Workout_Name_Duplicate()
	{
		var command = _fixture.Build<AcceptSharedWorkout>().With(x => x.NewName, (string)null).Create();
		var workoutName = _fixture.Create<string>();
		var workouts = Enumerable.Range(0, Globals.MaxWorkouts / 2)
			.Select(_ => _fixture.Build<WorkoutInfo>().With(y => y.WorkoutName, workoutName).Create())
			.ToList();
		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.Create();

		var sharedWorkout = SharedWorkoutHelper.GetSharedWorkout(command.UserId);
		sharedWorkout.WorkoutName = workoutName;

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync(sharedWorkout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<AlreadyExistsException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Max_Free_Exercises_Exceeded()
	{
		var command = _fixture.Build<AcceptSharedWorkout>().With(x => x.NewName, (string)null).Create();

		var sharedWorkout = SharedWorkoutHelper.GetSharedWorkout(command.UserId);

		var workouts = Enumerable.Range(0, Globals.MaxFreeWorkouts / 2)
			.Select(_ => _fixture.Create<WorkoutInfo>())
			.ToList();

		var exercisesToCreate = (Globals.MaxFreeExercises - sharedWorkout.DistinctExercises.Count) + 1;
		var ownedExercises = Enumerable.Range(0, exercisesToCreate)
			.Select(_ => _fixture.Create<OwnedExercise>())
			.ToList();

		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.With(x => x.PremiumToken, (string)null)
			.With(x => x.Exercises, ownedExercises)
			.Create();

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync(sharedWorkout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<MaxLimitException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Max_Exercises_Exceeded()
	{
		var command = _fixture.Build<AcceptSharedWorkout>().With(x => x.NewName, (string)null).Create();

		var sharedWorkout = SharedWorkoutHelper.GetSharedWorkout(command.UserId);

		var workouts = Enumerable.Range(0, Globals.MaxFreeWorkouts / 2)
			.Select(_ => _fixture.Create<WorkoutInfo>())
			.ToList();

		var exercisesToCreate = (Globals.MaxExercises - sharedWorkout.DistinctExercises.Count) + 1;
		var ownedExercises = Enumerable.Range(0, exercisesToCreate)
			.Select(_ => _fixture.Create<OwnedExercise>())
			.ToList();

		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, workouts)
			.With(x => x.Exercises, ownedExercises)
			.Create();

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync(sharedWorkout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await Assert.ThrowsAsync<MaxLimitException>(() => _handler.HandleAsync(command));
	}
}