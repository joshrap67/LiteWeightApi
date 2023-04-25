using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Exceptions.BaseExceptions;
using LiteWeightAPI.Imports;

namespace LiteWeightAPI.Services.Validation;

public interface ICommonValidator
{
	void AuthenticatedUserExists(User authenticatedUser);
	void UserExists(User user, string username);
	void EnsureWorkoutOwnership(string username, Workout workout);
	void EnsureSharedWorkoutResourceExists(SharedWorkout workout);
	void EnsureValidWorkoutName(string workoutName, User user);
}

public class CommonValidator : ICommonValidator
{
	public void AuthenticatedUserExists(User authenticatedUser)
	{
		if (authenticatedUser == null)
		{
			throw new UserNotFoundException("Authenticated user not found"); // todo this might be excessive
		}
	}

	public void UserExists(User user, string username)
	{
		if (user == null)
		{
			throw new UserNotFoundException($"User {username} not found");
		}
	}

	public void EnsureWorkoutOwnership(string username, Workout workout)
	{
		if (workout.Creator != username)
		{
			throw new ForbiddenException("User does not have permissions to modify workout");
		}
	}

	public void EnsureSharedWorkoutResourceExists(SharedWorkout workout)
	{
		if (workout == null)
		{
			throw new ResourceNotFoundException("Shared workout not found");
		}
	}

	public void EnsureValidWorkoutName(string workoutName, User user)
	{
		if (workoutName.Length > Globals.MaxWorkoutNameLength)
		{
			// todo put in request
			throw new Exception("Workout name is too long");
		}

		if (user.Workouts.Any(x => x.WorkoutName == workoutName))
		{
			throw new DuplicateFoundException("Workout name already exists");
		}
	}
}