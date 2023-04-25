using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime.CredentialManagement;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightAPI.Domain;

public interface IRepository
{
	Task<User> GetUser(string userId);
	Task<User> CreateUser(User user);
	Task<User> PutUser(User user);
	Task DeleteUser(string userId);
	Task<Workout> GetWorkout(string workoutId);
	Task<Workout> CreateWorkout(Workout workout);
	Task<Workout> PutWorkout(Workout workout);
	Task DeleteWorkout(string workoutId);
	Task<SharedWorkout> GetSharedWorkout(string sharedWorkoutId);
	Task DeleteSharedWorkout(string workoutId);

	Task ExecuteBatchWrite(IList<Workout> workoutsToPut = null, IList<User> usersToPut = null,
		IList<SharedWorkout> sharedWorkoutsToPut = null, IList<Workout> workoutsToDelete = null,
		IList<User> usersToDelete = null, IList<SharedWorkout> sharedWorkoutsToDelete = null);
}

public class Repository : IRepository
{
	private readonly Table _usersTable;
	private readonly Table _workoutsTable;
	private readonly Table _sharedWorkoutsTable;

	public Repository(IAmazonDynamoDB dynamoDbClient)
	{
		_usersTable = Table.LoadTable(dynamoDbClient, "users", true);
		_workoutsTable = Table.LoadTable(dynamoDbClient, "workouts", true);
		_sharedWorkoutsTable = Table.LoadTable(dynamoDbClient, "sharedWorkouts", true);
	}

	public async Task ExecuteBatchWrite(IList<Workout> workoutsToPut = null, IList<User> usersToPut = null,
		IList<SharedWorkout> sharedWorkoutsToPut = null, IList<Workout> workoutsToDelete = null,
		IList<User> usersToDelete = null, IList<SharedWorkout> sharedWorkoutsToDelete = null)
	{
		// 25 item limit on these
		var workoutBatchWrite = _workoutsTable.CreateBatchWrite();
		foreach (var workout in workoutsToDelete ?? new List<Workout>())
		{
			workoutBatchWrite.AddItemToDelete(DocumentHelper.Serialize(workout));
		}

		foreach (var workout in workoutsToPut ?? new List<Workout>())
		{
			workoutBatchWrite.AddDocumentToPut(DocumentHelper.Serialize(workout));
		}

		var userBatchWrite = _usersTable.CreateBatchWrite();
		foreach (var user in usersToPut ?? new List<User>())
		{
			userBatchWrite.AddDocumentToPut(DocumentHelper.Serialize(user));
		}

		foreach (var user in usersToDelete ?? new List<User>())
		{
			userBatchWrite.AddItemToDelete(DocumentHelper.Serialize(user));
		}

		var sharedWorkoutBatchWrite = _sharedWorkoutsTable.CreateBatchWrite();
		foreach (var sharedWorkout in sharedWorkoutsToPut ?? new List<SharedWorkout>())
		{
			sharedWorkoutBatchWrite.AddDocumentToPut(DocumentHelper.Serialize(sharedWorkout));
		}

		foreach (var sharedWorkout in sharedWorkoutsToDelete ?? new List<SharedWorkout>())
		{
			sharedWorkoutBatchWrite.AddItemToDelete(DocumentHelper.Serialize(sharedWorkout));
		}

		var superBatch = new MultiTableDocumentBatchWrite(workoutBatchWrite, userBatchWrite, sharedWorkoutBatchWrite);
		await superBatch.ExecuteAsync();
	}

	public async Task<User> GetUser(string userId)
	{
		var userDocument = await _usersTable.GetItemAsync(new Primitive(userId));
		return userDocument == null ? null : DocumentHelper.Deserialize<User>(userDocument);
	}

	public async Task<User> CreateUser(User user)
	{
		var document = DocumentHelper.Serialize(user);
		var createdDocument = await _usersTable.PutItemAsync(document);
		return DocumentHelper.Deserialize<User>(createdDocument);
	}

	public async Task<User> PutUser(User user)
	{
		var document = DocumentHelper.Serialize(user);
		var createdDocument = await _usersTable.PutItemAsync(document);
		return DocumentHelper.Deserialize<User>(createdDocument);
	}

	public async Task DeleteUser(string userId)
	{
		await _usersTable.DeleteItemAsync(userId);
	}

	public async Task<Workout> GetWorkout(string workoutId)
	{
		var workoutDocument = await _workoutsTable.GetItemAsync(new Primitive(workoutId));
		return workoutDocument == null ? null : DocumentHelper.Deserialize<Workout>(workoutDocument);
	}

	public async Task<Workout> CreateWorkout(Workout workout)
	{
		var document = DocumentHelper.Serialize(workout);
		var createdDocument = await _workoutsTable.PutItemAsync(document);
		return DocumentHelper.Deserialize<Workout>(createdDocument);
	}

	public async Task<Workout> PutWorkout(Workout workout)
	{
		var document = DocumentHelper.Serialize(workout);
		var createdDocument = await _workoutsTable.PutItemAsync(document);
		return DocumentHelper.Deserialize<Workout>(createdDocument);
	}

	public async Task DeleteWorkout(string workoutId)
	{
		await _workoutsTable.DeleteItemAsync(workoutId);
	}

	public async Task<SharedWorkout> GetSharedWorkout(string sharedWorkoutId)
	{
		var document = await _sharedWorkoutsTable.GetItemAsync(sharedWorkoutId);
		return document == null ? null : DocumentHelper.Deserialize<SharedWorkout>(document);
	}

	public async Task DeleteSharedWorkout(string workoutId)
	{
		await _sharedWorkoutsTable.DeleteItemAsync(workoutId);
	}
}