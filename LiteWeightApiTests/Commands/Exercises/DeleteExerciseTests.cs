using AutoFixture;
using LiteWeightAPI.Commands.Exercises.DeleteExercise;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using Moq;

namespace LiteWeightApiTests.Commands.Exercises;

public class DeleteExerciseTests
{
	private readonly DeleteExerciseHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public DeleteExerciseTests()
	{
		_mockRepository = new Mock<IRepository>();
		_handler = new DeleteExerciseHandler(_mockRepository.Object);
	}

	[Fact]
	public async Task Should_Delete_Exercise()
	{
		var command = _fixture.Create<DeleteExercise>();
		var exercise = _fixture.Build<OwnedExercise>()
			.With(x => x.Id, command.ExerciseId)
			.Create();
		var exercises = new List<OwnedExercise> { exercise };

		var routine = _fixture.Build<Routine>()
			.With(x => x.Weeks, new List<RoutineWeek>
			{
				new()
				{
					Days = new List<RoutineDay>
					{
						new()
						{
							Exercises = new List<RoutineExercise>
							{
								_fixture.Build<RoutineExercise>().With(x => x.ExerciseId, exercise.Id).Create()
							}
						}
					}
				}
			}).Create();

		var workout = _fixture.Build<Workout>()
			.With(x => x.Routine, routine)
			.Create();

		var user = _fixture.Build<User>()
			.With(x => x.Exercises, exercises)
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);
		_mockRepository
			.Setup(x => x.GetWorkout(It.Is<string>(y => exercise.Workouts.Any(z => z.WorkoutId == y))))
			.ReturnsAsync(workout);

		await _handler.HandleAsync(command);

		Assert.True(user.Exercises.All(x => x.Id != exercise.Id));
		// workout should no longer contain exercise
		Assert.True(workout.Routine.Weeks.SelectMany(x => x.Days).SelectMany(x => x.Exercises)
			.All(x => x.ExerciseId != exercise.Id));
	}

	[Fact]
	public async Task Should_Throw_Exception_Exercise_Not_Found()
	{
		var command = _fixture.Create<DeleteExercise>();
		command.ExerciseId = _fixture.Create<string>();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(_fixture.Create<User>());

		await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.HandleAsync(command));
	}
}