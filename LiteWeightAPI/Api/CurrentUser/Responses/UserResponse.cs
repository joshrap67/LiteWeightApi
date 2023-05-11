using LiteWeightAPI.Api.Exercises.Responses;

namespace LiteWeightAPI.Api.CurrentUser.Responses;

public class UserResponse
{
	/// <summary>
	/// Unique identifier of the user
	/// </summary>
	/// <example>f1e03cd1-e62c-4a53-84ed-498c72776fc2</example>
	public string Id { get; set; }

	/// <summary>
	/// Username of the user.
	/// </summary>
	/// <example>barbell_bill</example>
	public string Username { get; set; }

	/// <summary>
	/// Email address of the user.
	/// </summary>
	/// <example>barbellB12@gmail.com</example>
	public string Email { get; set; }

	/// <summary>
	/// File path of the user's icon
	/// </summary>
	/// <example>0f1d96c3-ca22-4657-9f9b-136bb4621985.jpg</example>
	public string Icon { get; set; } // todo backend should prepend the domain

	/// <summary>
	/// Amazon Reference Number (ARN) for the user's push notification endpoint in SNS.
	/// </summary>
	/// <example>arn:aws:sns:us-east-1:438338746171:endpoint/GCM/LiteWeight/1eacec71-fb64-39ae-a638-bc21bb2ee062</example>
	public string PushEndpointArn { get; set; }

	/// <summary>
	/// Token indicating the user has purchased LiteWeight premium. Current not used.
	/// </summary>
	/// <example>a704f441-8ee3-471b-ac8a-abb0b7d8249a</example>
	public string PremiumToken { get; set; }

	/// <summary>
	/// Workout Id that the user is currently on.
	/// </summary>
	/// <example>b209f062-36fa-4089-aca0-31df4815744f</example>
	public string CurrentWorkoutId { get; set; }

	/// <summary>
	/// Total number of workouts sent.
	/// </summary>
	/// <example>14</example>
	public int WorkoutsSent { get; set; }

	/// <summary>
	/// Preferences of the user.
	/// </summary>
	public UserPreferencesResponse UserPreferences { get; set; }

	/// <summary>
	/// List of workouts the user owns.
	/// </summary>
	public IList<WorkoutInfoResponse> Workouts { get; set; } = new List<WorkoutInfoResponse>();

	/// <summary>
	/// List of exercises the user owns.
	/// </summary>
	public IList<OwnedExerciseResponse> Exercises { get; set; } = new List<OwnedExerciseResponse>();

	/// <summary>
	/// List of friends for the user.
	/// </summary>
	public IList<FriendResponse> Friends { get; set; } = new List<FriendResponse>();

	/// <summary>
	/// List of pending friend requests for the user.
	/// </summary>
	public IList<FriendRequestResponse> FriendRequests { get; set; } = new List<FriendRequestResponse>();

	/// <summary>
	/// List of workouts sent to the user.
	/// </summary>
	public IList<SharedWorkoutInfoResponse> ReceivedWorkouts { get; set; } = new List<SharedWorkoutInfoResponse>();
}