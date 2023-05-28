using AutoMapper;
using LiteWeightAPI.Api.Self.Requests;
using LiteWeightAPI.Api.Workouts.Requests;
using LiteWeightAPI.Api.Workouts.Responses;
using LiteWeightAPI.Commands;
using LiteWeightAPI.Commands.Workouts;
using LiteWeightAPI.Commands.Workouts.CopyWorkout;
using LiteWeightAPI.Commands.Workouts.CreateWorkout;
using LiteWeightAPI.Commands.Workouts.DeleteWorkout;
using LiteWeightAPI.Commands.Workouts.DeleteWorkoutAndSetCurrent;
using LiteWeightAPI.Commands.Workouts.GetWorkout;
using LiteWeightAPI.Commands.Workouts.RenameWorkout;
using LiteWeightAPI.Commands.Workouts.ResetStatistics;
using LiteWeightAPI.Commands.Workouts.RestartWorkout;
using LiteWeightAPI.Commands.Workouts.UpdateRoutine;
using LiteWeightAPI.Commands.Workouts.UpdateWorkoutProgress;
using LiteWeightAPI.Errors.Attributes;
using LiteWeightAPI.Errors.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LiteWeightAPI.Api.Workouts;

[Route("workouts")]
[ApiController]
public class WorkoutsController : BaseController
{
	private readonly ICommandDispatcher _dispatcher;
	private readonly IMapper _mapper;

	public WorkoutsController(Serilog.ILogger logger, ICommandDispatcher dispatcher, IMapper mapper) : base(logger)
	{
		_dispatcher = dispatcher;
		_mapper = mapper;
	}

	/// <summary>Create Workout</summary>
	/// <remarks>Creates a workout and adds it to the authenticated user's list of workouts.</remarks>
	[HttpPost]
	[InvalidRequest, InvalidRoutine, MaxLimit]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	public async Task<ActionResult<UserAndWorkoutResponse>> CreateWorkout(CreateWorkoutRequest request)
	{
		var command = _mapper.Map<CreateWorkout>(request);
		command.UserId = CurrentUserId;

		var response = await _dispatcher.DispatchAsync<CreateWorkout, UserAndWorkoutResponse>(command);
		return response;
	}

	/// <summary>Get Workout</summary>
	/// <remarks>Fetches a workout assuming it belongs to the authenticated user.</remarks>
	[HttpGet("{workoutId}")]
	[ProducesResponseType(typeof(WorkoutResponse), 200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<WorkoutResponse>> GetWorkout(string workoutId)
	{
		var response = await _dispatcher.DispatchAsync<GetWorkout, WorkoutResponse>(new GetWorkout
		{
			WorkoutId = workoutId,
			UserId = CurrentUserId
		});
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
		var response = await _dispatcher.DispatchAsync<CopyWorkout, UserAndWorkoutResponse>(new CopyWorkout()
		{
			UserId = CurrentUserId,
			NewName = request.NewName,
			WorkoutId = workoutId
		});
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
		var response = await _dispatcher.DispatchAsync<UpdateRoutine, UserAndWorkoutResponse>(new UpdateRoutine
		{
			Routine = _mapper.Map<SetRoutine>(request),
			UserId = CurrentUserId,
			WorkoutId = workoutId
		});
		return response;
	}

	/// <summary>Update Workout Progress</summary>
	/// <remarks>Updates the specified workout's progress.</remarks>
	[HttpPut("{workoutId}/update-progress")]
	[InvalidRequest, InvalidRoutine]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> UpdateProgress(string workoutId, UpdateWorkoutProgressRequest request)
	{
		var command = new UpdateWorkoutProgress
		{
			WorkoutId = workoutId,
			UserId = CurrentUserId,
			Routine = _mapper.Map<SetRoutine>(request.Routine),
			CurrentWeek = request.CurrentWeek,
			CurrentDay = request.CurrentDay
		};
		await _dispatcher.DispatchAsync<UpdateWorkoutProgress, bool>(command);
		return Ok();
	}

	/// <summary>Reset Statistics</summary>
	/// <remarks>Resets the statistics for a given workout, if it exists.</remarks>
	[HttpPut("{workoutId}/reset-statistics")]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> ResetStatistics(string workoutId)
	{
		await _dispatcher.DispatchAsync<ResetStatistics, bool>(new ResetStatistics
		{
			UserId = CurrentUserId,
			WorkoutId = workoutId
		});
		return Ok();
	}

	/// <summary>Restart Workout</summary>
	/// <remarks>
	/// Restarts the workout to have all exercises set to incomplete, and updates the statistics of the authenticated user using the state of the workout before it was restarted.
	/// <br/><br/>If enabled on the authenticated user's preferences, the default weights of any completed exercises will be updated if their completed weight is greater than the current default weight.
	/// </remarks>
	[HttpPost("{workoutId}/restart")]
	[InvalidRequest]
	[ProducesResponseType(typeof(UserAndWorkoutResponse), 200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult<UserAndWorkoutResponse>> RestartWorkout(string workoutId,
		RestartWorkoutRequest request)
	{
		var response = await _dispatcher.DispatchAsync<RestartWorkout, UserAndWorkoutResponse>(new RestartWorkout
		{
			UserId = CurrentUserId,
			WorkoutId = workoutId,
			Routine = _mapper.Map<SetRoutine>(request.Routine)
		});
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
		await _dispatcher.DispatchAsync<RenameWorkout, bool>(new RenameWorkout
		{
			UserId = CurrentUserId,
			WorkoutId = workoutId,
			NewName = request.NewName
		});
		return Ok();
	}

	/// <summary>Delete Workout</summary>
	/// <remarks>Deletes a given workout. Removes it from the authenticated user's list of workouts, and from the list of workouts on the exercises of the deleted workout.</remarks>
	/// <param name="workoutId">Id of the workout to delete</param>
	[HttpDelete("{workoutId}")]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> DeleteWorkout(string workoutId)
	{
		await _dispatcher.DispatchAsync<DeleteWorkout, bool>(new DeleteWorkout
		{
			UserId = CurrentUserId,
			WorkoutId = workoutId
		});
		return Ok();
	}

	/// <summary>Delete Workout and Set Current</summary>
	/// <remarks>
	/// Deletes a given workout. Removes it from the authenticated user's list of workouts, and from the list of workouts on the exercises of the deleted workout.
	/// <br/><br/> Also sets the current workout to the specified workout, if it exists.
	/// </remarks>
	/// <param name="workoutId">Id of the workout to delete</param>
	/// <param name="request">Request</param>
	[HttpPut("{workoutId}/delete-and-set-current")]
	[WorkoutNotFound]
	[ProducesResponseType(200)]
	[ProducesResponseType(typeof(BadRequestResponse), 400)]
	[ProducesResponseType(typeof(ResourceNotFoundResponse), 404)]
	public async Task<ActionResult> DeleteWorkoutAndSetCurrent(string workoutId, SetCurrentWorkoutRequest request)
	{
		// combining these two actions since it is a really bad state to be in on the app atm if the delete succeeds and the set current workout does not. So need a transactional request
		await _dispatcher.DispatchAsync<DeleteWorkoutAndSetCurrent, bool>(new DeleteWorkoutAndSetCurrent
		{
			UserId = CurrentUserId,
			WorkoutToDeleteId = workoutId,
			CurrentWorkoutId = request.WorkoutId
		});
		return Ok();
	}
}