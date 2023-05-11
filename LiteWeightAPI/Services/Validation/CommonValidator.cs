using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;

namespace LiteWeightAPI.Services.Validation;

public interface ICommonValidator
{
	void UserExists(User user, string userId);
	void WorkoutExists(Workout workout);
	void EnsureWorkoutOwnership(string userId, Workout workout);
	void EnsureSharedWorkoutOwnership(string userId, SharedWorkout workout);
	void SharedWorkoutExists(SharedWorkout workout);
	void ValidWorkoutName(string workoutName, User user);
}

public class CommonValidator : ICommonValidator
{
	public void UserExists(User user, string userId)
	{
		if (user == null)
		{
			throw new UserNotFoundException($"User {userId} not found");
		}
	}

	public void WorkoutExists(Workout workout)
	{
		if (workout == null)
		{
			throw new ResourceNotFoundException("Workout");
		}
	}

	public void EnsureWorkoutOwnership(string userId, Workout workout)
	{
		if (workout.CreatorId != userId)
		{
			throw new ForbiddenException("User does not have permissions to access workout");
		}
	}

	public void EnsureSharedWorkoutOwnership(string userId, SharedWorkout sharedWorkout)
	{
		if (sharedWorkout.RecipientId != userId)
		{
			throw new ForbiddenException("User does not have permissions to access shared workout");
		}
	}

	public void SharedWorkoutExists(SharedWorkout workout)
	{
		if (workout == null)
		{
			throw new ResourceNotFoundException("Shared workout");
		}
	}

	public void ValidWorkoutName(string workoutName, User user)
	{
		if (user.Workouts.Any(x => x.WorkoutName == workoutName))
		{
			// todo "AlreadyExists"?
			throw new AlreadyExistsException("Workout name already exists");
		}
	}
}