using LiteWeightAPI.Api.Common.Responses;
using LiteWeightAPI.Api.Workouts.Requests;
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
	/// <remarks>Creates a workout and adds it to the authenticated user's list of workouts.</remarks>
	[HttpPost]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 200)]
	public async Task<ActionResult<UserAndWorkoutResponse>> CreateWorkout(CreateWorkoutRequest request)
	{
		var response = await _workoutService.CreateWorkout(request, UserId);
		return response;
	}

	/// <summary>Switch Workout</summary>
	/// <remarks>Switches the authenticated user's workout to the specified one, and updates the current workout if specified.</remarks>
	[HttpPut("switch")]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 200)]
	public async Task<ActionResult<UserAndWorkoutResponse>> SwitchWorkout(SwitchWorkoutRequest request)
	{
		var response = await _workoutService.SwitchWorkout(request, UserId);
		return response;
	}

	/// <summary>Copy Workout</summary>
	/// <remarks>Copies a workout as a new workout.</remarks>
	[HttpPost("copy")]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 200)]
	public async Task<ActionResult<UserAndWorkoutResponse>> CopyWorkout(CopyWorkoutRequest request)
	{
		// todo this should not switch, that is too much in one method. need to update frontend to handle that
		var response = await _workoutService.CopyWorkout(request, UserId);
		return response;
	}

	/// <summary>Set Routine</summary>
	/// <remarks>Sets the routine of a given workout.</remarks>
	[HttpPut("{workoutId}/set-routine")]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 200)]
	public async Task<ActionResult<UserAndWorkoutResponse>> SetRoutine(SetRoutineRequest request, string workoutId)
	{
		var response = await _workoutService.SetRoutine(request, workoutId, UserId);
		return response;
	}

	/// <summary>Update Workout</summary>
	/// <remarks>Updates the specified workout.</remarks>
	[HttpPut("update")]
	public async Task<ActionResult> UpdateWorkout(UpdateWorkoutRequest request)
	{
		await _workoutService.UpdateWorkout(request, UserId);
		return Ok();
	}

	/// <summary>Restart Workout</summary>
	/// <remarks>Restarts the workout to have all exercises set to incomplete, and updates the statistics of the authenticated user using the state of the workout before it was restarted.</remarks>
	[HttpPut("restart")]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 200)]
	public async Task<ActionResult<UserAndWorkoutResponse>> RestartWorkout(RestartWorkoutRequest request)
	{
		var response = await _workoutService.RestartWorkout(request, UserId);
		return response;
	}

	/// <summary>Rename Workout</summary>
	/// <remarks>Renames a given workout. Name must be unique.</remarks>
	[HttpPut("{workoutId}/rename")]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 200)]
	public async Task<ActionResult<UserAndWorkoutResponse>> RenameWorkout(RenameWorkoutRequest request,
		string workoutId)
	{
		var response = await _workoutService.RenameWorkout(request, workoutId, UserId);
		return response;
	}

	/// <summary>Delete Workout</summary>
	/// <remarks>Deletes a given workout. Removes it from the authenticated user's list of workouts, and from the list of workouts on the exercises of the deleted workout.</remarks>
	[HttpDelete("{workoutId}")]
	public async Task<ActionResult> DeleteWorkout(string workoutId)
	{
		// todo this should not switch, that is too much in one method. need to update frontend to handle that
		await _workoutService.DeleteWorkout(workoutId, UserId);
		return Ok();
	}
}