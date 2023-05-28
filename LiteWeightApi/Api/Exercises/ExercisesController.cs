using AutoMapper;
using LiteWeightAPI.Api.Exercises.Requests;
using LiteWeightAPI.Api.Exercises.Responses;
using LiteWeightAPI.Commands;
using LiteWeightAPI.Commands.Exercises.AddExercise;
using LiteWeightAPI.Commands.Exercises.DeleteExercise;
using LiteWeightAPI.Commands.Exercises.UpdateExercise;
using LiteWeightAPI.Errors.Attributes;
using LiteWeightAPI.Errors.Responses;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace LiteWeightAPI.Api.Exercises;

[Route("exercises")]
[ApiController]
public class ExercisesController : BaseController
{
	private readonly ICommandDispatcher _commandDispatcher;
	private readonly IMapper _mapper;

	public ExercisesController(ILogger logger, ICommandDispatcher commandDispatcher, IMapper mapper)
		: base(logger)
	{
		_commandDispatcher = commandDispatcher;
		_mapper = mapper;
	}

	/// <summary>Create Exercise</summary>
	/// <remarks>Creates an exercise to be owned by the authenticated user. The name of the exercise must not already exist for the user.</remarks>
	[HttpPost]
	[AlreadyExists, InvalidRequest, MaxLimit]
	[ProducesResponseType(typeof(OwnedExerciseResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult<OwnedExerciseResponse>> CreateExercise(SetExerciseRequest request)
	{
		var command = _mapper.Map<AddExercise>(request);
		command.UserId = CurrentUserId;

		var response = await _commandDispatcher.DispatchAsync<AddExercise, OwnedExerciseResponse>(command);
		return response;
	}

	/// <summary>Update Exercise</summary>
	/// <remarks>Updates an exercise owned by the authenticated user, if it exists. Note that any new name for the exercise must not already exist.</remarks>
	/// <param name="exerciseId">Id of the exercise to update</param>
	/// <param name="request">Request</param>
	[HttpPut("{exerciseId}")]
	[AlreadyExists, InvalidRequest]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> UpdateExercise(string exerciseId, SetExerciseRequest request)
	{
		var command = _mapper.Map<UpdateExercise>(request);
		command.UserId = CurrentUserId;
		command.ExerciseId = exerciseId;

		await _commandDispatcher.DispatchAsync<UpdateExercise, bool>(command);
		return Ok();
	}

	/// <summary>Delete Exercise</summary>
	/// <remarks>Removes an exercise owned by the authenticated user. Removes the deleted exercise from any workout it was a part of.</remarks>
	/// <param name="exerciseId">Id of the exercise to delete</param>
	[HttpDelete("{exerciseId}")]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> DeleteExercise(string exerciseId)
	{
		var command = new DeleteExercise { ExerciseId = exerciseId, UserId = CurrentUserId };
		await _commandDispatcher.DispatchAsync<DeleteExercise, bool>(command);
		return Ok();
	}
}