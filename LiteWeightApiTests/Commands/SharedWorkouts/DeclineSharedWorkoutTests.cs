using LiteWeightAPI.Commands.SharedWorkouts.DeclineSharedWorkout;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightApiTests.TestHelpers;

namespace LiteWeightApiTests.Commands.SharedWorkouts;

public class DeclineSharedWorkoutTests : BaseTest
{
	private readonly DeclineSharedWorkoutHandler _handler;
	private readonly Mock<IRepository> _mockRepository;

	public DeclineSharedWorkoutTests()
	{
		_mockRepository = new Mock<IRepository>();
		_handler = new DeclineSharedWorkoutHandler(_mockRepository.Object);
	}

	[Fact]
	public async Task Should_Decline_Workout()
	{
		var command = Fixture.Create<DeclineSharedWorkout>();
		var sharedWorkout = SharedWorkoutHelper.GetSharedWorkout(command.UserId);
		var user = Fixture.Build<User>()
			.With(x => x.Id, command.UserId)
			.With(x => x.ReceivedWorkouts, new List<SharedWorkoutInfo>
			{
				Fixture.Build<SharedWorkoutInfo>().With(x => x.SharedWorkoutId, command.SharedWorkoutId).Create()
			})
			.Create();

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync(sharedWorkout);

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		await _handler.HandleAsync(command);
		Assert.True(user.ReceivedWorkouts.All(x => x.SharedWorkoutId != command.SharedWorkoutId));
	}

	[Fact]
	public async Task Should_Throw_Exception_Workout_Does_Not_Exist()
	{
		var command = Fixture.Create<DeclineSharedWorkout>();

		_mockRepository
			.Setup(x => x.GetSharedWorkout(It.Is<string>(y => y == command.SharedWorkoutId)))
			.ReturnsAsync((SharedWorkout)null);

		await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Missing_Permissions_Workout()
	{
		var command = Fixture.Create<DeclineSharedWorkout>();
		var user = Fixture.Create<User>();

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