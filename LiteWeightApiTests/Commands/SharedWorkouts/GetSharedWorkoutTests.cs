using AutoMapper;
using LiteWeightAPI.Api.Exercises;
using LiteWeightAPI.Commands.SharedWorkouts.GetSharedWorkout;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;

namespace LiteWeightApiTests.Commands.SharedWorkouts;

public class GetSharedWorkoutTests
{
	private readonly GetSharedWorkoutHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public GetSharedWorkoutTests()
	{
		var configuration = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(ExercisesController)); });
		var mapper = new Mapper(configuration);

		_mockRepository = new Mock<IRepository>();
		_handler = new GetSharedWorkoutHandler(_mockRepository.Object, mapper);
	}

	[Fact]
	public async Task Should_Get_Workout()
	{
		var command = _fixture.Create<GetSharedWorkout>();
		var sharedWorkout = SharedWorkoutHelper.GetSharedWorkout(command.UserId, command.SharedWorkoutId);

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync(sharedWorkout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(_fixture.Create<User>());

		var response = await _handler.HandleAsync(command);
		Assert.Equal(command.SharedWorkoutId, response.Id);
	}

	[Fact]
	public async Task Should_Throw_Exception_Workout_Does_Not_Exist()
	{
		var command = _fixture.Create<GetSharedWorkout>();

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync((SharedWorkout)null!);

		await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Missing_Permissions_Workout()
	{
		var command = _fixture.Create<GetSharedWorkout>();
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
}