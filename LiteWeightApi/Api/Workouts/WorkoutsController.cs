using LiteWeightAPI.Api.Workouts.Requests;
using LiteWeightAPI.Api.Workouts.Responses;
using LiteWeightAPI.Errors.Attributes;
using LiteWeightAPI.Errors.Responses;
using LiteWeightAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiteWeightAPI.Api.Workouts;

[Route("workouts")]
[ApiController]
public class WorkoutsController : BaseController
{
	private readonly IWorkoutService _workoutService;

	public WorkoutsController(Serilog.ILogger logger, IWorkoutService workoutService) : base(logger)
	{
		_workoutService = workoutService;
	}

	/// <summary>Create Workout</summary>
	/// <remarks>Creates a workout and adds it to the current user's list of workouts.</remarks>
	[HttpPost]
	[InvalidRequest, InvalidRoutine, MaxLimit]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult<UserAndWorkoutResponse>> CreateWorkout(CreateWorkoutRequest request)
	{
		var response = await _workoutService.CreateWorkout(request, CurrentUserId);
		return response;
	}

	/// <summary>Get Workout</summary>
	/// <remarks>Gets a workout assuming it belongs to the authenticated user</remarks>
	[HttpGet("{workoutId}")]
	[ProducesResponseType(typeof(WorkoutResponse), 200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<WorkoutResponse>> GetWorkout(string workoutId)
	{
		var response = await _workoutService.GetWorkout(workoutId, CurrentUserId);
		return response;
	}

	/// <summary>Copy Workout</summary>
	/// <remarks>Copies a workout as a new workout.</remarks>
	[HttpPost("{workoutId}/copy")]
	[InvalidRequest, AlreadyExists, MaxLimit]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<UserAndWorkoutResponse>> CopyWorkout(string workoutId, CopyWorkoutRequest request)
	{
		var response = await _workoutService.CopyWorkout(request, workoutId, CurrentUserId);
		return response;
	}

	/// <summary>Set Routine</summary>
	/// <remarks>Sets the routine of a given workout.</remarks>
	[HttpPut("{workoutId}/routine")]
	[InvalidRequest, InvalidRoutine]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<UserAndWorkoutResponse>> SetRoutine(SetRoutineRequest request, string workoutId)
	{
		var response = await _workoutService.SetRoutine(request, workoutId, CurrentUserId);
		return response;
	}

	/// <summary>Update Workout Progress</summary>
	/// <remarks>Updates the specified workout progress.</remarks>
	[HttpPut("{workoutId}/update-progress")]
	[InvalidRequest, InvalidRoutine]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> UpdateProgress(string workoutId, UpdateWorkoutProgressRequest request)
	{
		await _workoutService.UpdateProgress(workoutId, request, CurrentUserId);
		return Ok();
	}

	/// <summary>Reset Statistics</summary>
	/// <remarks>Resets the statistics for a given workout, if it exists.</remarks>
	[HttpPut("{workoutId}/reset-statistics")]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> ResetStatistics(string workoutId)
	{
		await _workoutService.ResetStatistics(workoutId, CurrentUserId);
		return Ok();
	}

	/// <summary>Restart Workout</summary>
	/// <remarks>
	/// Restarts the workout to have all exercises set to incomplete, and updates the statistics of the authenticated user using the state of the workout before it was restarted.
	/// <br/>If enabled on the current user's preferences, the default weights of any completed exercises will be updated if their completed weight is greater than the current default weight.
	/// </remarks>
	[HttpPost("{workoutId}/restart")]
	[InvalidRequest]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<UserAndWorkoutResponse>> RestartWorkout(string workoutId,
		RestartWorkoutRequest request)
	{
		var response = await _workoutService.RestartWorkout(workoutId, request, CurrentUserId);
		return response;
	}

	/// <summary>Rename Workout</summary>
	/// <remarks>Renames a given workout. Name must be unique.</remarks>
	[HttpPut("{workoutId}/rename")]
	[InvalidRequest, AlreadyExists]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> RenameWorkout(string workoutId, RenameWorkoutRequest request)
	{
		await _workoutService.RenameWorkout(request, workoutId, CurrentUserId);
		return Ok();
	}

	/// <summary>Delete Workout</summary>
	/// <remarks>Deletes a given workout. Removes it from the authenticated user's list of workouts, and from the list of workouts on the exercises of the deleted workout.</remarks>
	[HttpDelete("{workoutId}")]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> DeleteWorkout(string workoutId)
	{
		await _workoutService.DeleteWorkout(workoutId, CurrentUserId);
		return Ok();
	}
}