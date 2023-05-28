using AutoFixture;
using AutoMapper;
using LiteWeightAPI.Api.Exercises;
using LiteWeightAPI.Commands.Exercises.AddExercise;
using LiteWeightAPI.Domain;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Imports;
using Moq;

namespace LiteWeightApiTests.Commands.Exercises;

public class AddExerciseTests
{
	private readonly AddExerciseHandler _handler;
	private readonly Mock<IRepository> _mockRepository;
	private readonly Fixture _fixture = new();

	public AddExerciseTests()
	{
		var configuration = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(ExercisesController)); });
		var mapper = new Mapper(configuration);

		_mockRepository = new Mock<IRepository>();
		_handler = new AddExerciseHandler(_mockRepository.Object, mapper);
	}

	[Fact]
	public async Task Should_Create_Exercise()
	{
		var command = _fixture.Create<AddExercise>();
		var exercises = Enumerable.Range(0, Globals.MaxPremiumExercises - 1)
			.Select(x => _fixture.Build<OwnedExercise>().Create())
			.ToList();

		var user = _fixture.Build<User>()
			.With(x => x.Exercises, exercises)
			.With(x => x.PremiumToken, _fixture.Create<string>())
			.Create();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(user);

		var createdExercise = await _handler.HandleAsync(command);

		Assert.True(createdExercise.Name == command.Name);
		Assert.True(createdExercise.DefaultDetails == command.DefaultDetails);
		Assert.True(Math.Abs(createdExercise.DefaultWeight - command.DefaultWeight) < 0.01);
		Assert.True(createdExercise.DefaultReps == command.DefaultReps);
		Assert.True(createdExercise.DefaultSets == command.DefaultSets);
		Assert.True(createdExercise.VideoUrl == command.VideoUrl);
		Assert.Equivalent(command.Focuses, createdExercise.Focuses);
		Assert.Contains(user.Exercises, x => x.Id == createdExercise.Id);
	}

	[Fact]
	public async Task Should_Throw_Exception_Name_Already_Exists()
	{
		var command = _fixture.Create<AddExercise>();
		command.Name = "Name";

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(_fixture.Build<User>()
				.With(x => x.Exercises, new List<OwnedExercise>
				{
					new() { Name = "Name" }
				})
				.Create()
			);

		await Assert.ThrowsAsync<AlreadyExistsException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Max_Limit_Free()
	{
		var command = _fixture.Create<AddExercise>();
		var exercises = Enumerable.Range(0, Globals.MaxFreeExercises + 1)
			.Select(x => _fixture.Build<OwnedExercise>().Create())
			.ToList();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(_fixture.Build<User>()
				.With(x => x.Exercises, exercises)
				.With(x => x.PremiumToken, () => null!)
				.Create());

		await Assert.ThrowsAsync<MaxLimitException>(() => _handler.HandleAsync(command));
	}

	[Fact]
	public async Task Should_Throw_Exception_Max_Limit()
	{
		var command = _fixture.Create<AddExercise>();
		var exercises = Enumerable.Range(0, Globals.MaxPremiumExercises + 1)
			.Select(x => _fixture.Build<OwnedExercise>().Create())
			.ToList();

		_mockRepository
			.Setup(x => x.GetUser(It.Is<string>(y => y == command.UserId)))
			.ReturnsAsync(_fixture.Build<User>()
				.With(x => x.Exercises, exercises)
				.With(x => x.PremiumToken, _fixture.Create<string>())
				.Create());

		await Assert.ThrowsAsync<MaxLimitException>(() => _handler.HandleAsync(command));
	}
}