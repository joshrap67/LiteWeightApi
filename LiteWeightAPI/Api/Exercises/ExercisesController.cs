using LiteWeightAPI.Api.Common.Responses.ErrorResponses;
using LiteWeightAPI.Api.Exercises.Requests;
using LiteWeightAPI.Api.Exercises.Responses;
using LiteWeightAPI.Errors.ErrorAttributes;
using LiteWeightAPI.Services;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace LiteWeightAPI.Api.Exercises;

[Route("exercises")]
[ApiController]
public class ExercisesController : BaseController
{
	private readonly IExercisesService _exercisesService;

	public ExercisesController(ILogger logger, IExercisesService exercisesService) : base(logger)
	{
		_exercisesService = exercisesService;
	}

	/// <summary>Create Exercise</summary>
	/// <remarks>Creates an exercise to be owned by the authenticated user. The name of the exercise must not already exist for the user.</remarks>
	[HttpPost]
	[AlreadyExists, InvalidRequest, MaxLimit]
	[ProducesResponseType(typeof(OwnedExerciseResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult<OwnedExerciseResponse>> CreateExercise(SetExerciseRequest request)
	{
		var response = await _exercisesService.CreateExercise(request, CurrentUserId);
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
		await _exercisesService.UpdateExercise(exerciseId, request, CurrentUserId);
		return Ok();
	}

	/// <summary>Delete Exercise</summary>
	/// <remarks>Removes an exercise owned by the authenticated user. Doing so removes it from any workout it was a part of.</remarks>
	/// <param name="exerciseId">Id of the exercise to delete</param>
	[HttpDelete("{exerciseId}")]
	public async Task<ActionResult> DeleteExercise(string exerciseId)
	{
		await _exercisesService.DeleteExercise(exerciseId, CurrentUserId);
		return Ok();
	}
}