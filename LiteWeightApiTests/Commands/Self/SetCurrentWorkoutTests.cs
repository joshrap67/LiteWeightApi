using LiteWeightAPI.Commands.Self.SetCurrentWorkout;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;
using NodaTime;

namespace LiteWeightApiTests.Commands.Self;

public class SetCurrentWorkoutTests
{
	private readonly SetCurrentWorkoutHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Mock<IClock> _mockClock;
	private readonly Fixture _fixture = new();

	public SetCurrentWorkoutTests()
	{
		_mockRepository = new Mock<IRepository>();
		_mockClock = new Mock<IClock>();
		_handler = new SetCurrentWorkoutHandler(_mockRepository.Object, _mockClock.Object);
	}

	[Fact]
	public async Task Should_Set_Current_Workout_Not_Null()
	{
		var command = _fixture.Create<SetCurrentWorkout>();

		var workoutInfo = _fixture.Build<WorkoutInfo>().With(x => x.WorkoutId, command.WorkoutId).Create();
		var user = _fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.Workouts, new List<WorkoutInfo> { workoutInfo })
			.Create();
		var instant = _fixture.Create<Instant>();
		_mockClock.Setup(x => x.GetCurrentInstant()).Returns(instant);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);

		Assert.Equal(command.WorkoutId, user.CurrentWorkoutId);
		Assert.Equal(instant, workoutInfo.LastSetAsCurrentUtc);
	}

	[Fact]
	public async Task Should_Set_Current_Workout_Null()
	{
		var command = _fixture.Build<SetCurrentWorkout>().With(x => x.WorkoutId, () => null!).Create();
		var user = _fixture.Build<User>().With(x => x.Id, command.UserId).Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);

		Assert.Null(user.CurrentWorkoutId);
	}

	[Fact]
	public async Task Should_Throw_Exception_Workout_Does_Not_Exist()
	{
		var command = _fixture.Create<SetCurrentWorkout>();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(_fixture.Create<User>());

		await Assert.ThrowsAsync<WorkoutNotFoundException>(() => _handler.HandleAsync(command));
	}
}