using Google.Cloud.Firestore;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;

namespace LiteWeightAPI.Domain;

public interface IRepository
{
	Task<User> GetUser(string userId);
	Task<User> GetUserByUsername(string username);
	Task<User> GetUserByEmail(string email);
	Task CreateUser(User user);
	Task PutUser(User user);
	Task DeleteUser(string userId);
	Task<Workout> GetWorkout(string workoutId);
	Task CreateWorkout(Workout workout);
	Task PutWorkout(Workout workout);
	Task DeleteWorkout(string workoutId);
	Task<SharedWorkout> GetSharedWorkout(string sharedWorkoutId);
	Task DeleteSharedWorkout(string workoutId);

	Task ExecuteBatchWrite(IList<Workout> workoutsToPut = null, IList<User> usersToPut = null,
		IList<SharedWorkout> sharedWorkoutsToPut = null, IList<Workout> workoutsToDelete = null,
		IList<User> usersToDelete = null, IList<SharedWorkout> sharedWorkoutsToDelete = null);
}

public class Repository : IRepository
{
	private const string WorkoutsCollection = "workouts"; // todo config
	private const string UsersCollection = "users";
	private const string SharedWorkoutCollection = "sharedWorkouts";

	private static FirestoreDb GetDb()
	{
		return FirestoreDb.Create("liteweight-faa1a"); // todo config
	}

	public async Task ExecuteBatchWrite(IList<Workout> workoutsToPut = null, IList<User> usersToPut = null,
		IList<SharedWorkout> sharedWorkoutsToPut = null, IList<Workout> workoutsToDelete = null,
		IList<User> usersToDelete = null, IList<SharedWorkout> sharedWorkoutsToDelete = null)
	{
		var db = GetDb();
		var batch = db.StartBatch();

		foreach (var workout in workoutsToDelete ?? new List<Workout>())
		{
			var workoutsRef = db.Collection(WorkoutsCollection).Document(workout.Id);
			batch.Delete(workoutsRef);
		}

		foreach (var workout in workoutsToPut ?? new List<Workout>())
		{
			var workoutsRef = db.Collection(WorkoutsCollection).Document(workout.Id);
			batch.Set(workoutsRef, workout);
		}

		foreach (var user in usersToDelete ?? new List<User>())
		{
			var usersRef = db.Collection(UsersCollection).Document(user.Id);
			batch.Delete(usersRef);
		}

		foreach (var user in usersToPut ?? new List<User>())
		{
			var usersRef = db.Collection(UsersCollection).Document(user.Id);
			batch.Set(usersRef, user);
		}

		foreach (var sharedWorkout in sharedWorkoutsToDelete ?? new List<SharedWorkout>())
		{
			var sharedWorkoutRef = db.Collection(SharedWorkoutCollection).Document(sharedWorkout.Id);
			batch.Delete(sharedWorkoutRef);
		}

		foreach (var sharedWorkout in sharedWorkoutsToPut ?? new List<SharedWorkout>())
		{
			var sharedWorkoutRef = db.Collection(SharedWorkoutCollection).Document(sharedWorkout.Id);
			batch.Set(sharedWorkoutRef, sharedWorkout);
		}

		await batch.CommitAsync();
	}

	public async Task<User> GetUser(string userId)
	{
		var db = GetDb();
		var docRef = db.Collection(UsersCollection).Document(userId);
		var snapshot = await docRef.GetSnapshotAsync();

		if (!snapshot.Exists) return null;
		var user = snapshot.ConvertTo<User>();
		return user;
	}

	public async Task<User> GetUserByUsername(string username)
	{
		var db = GetDb();
		var citiesRef = db.Collection(UsersCollection);
		var query = citiesRef.WhereEqualTo("username", username);
		var querySnapshot = await query.GetSnapshotAsync();

		var user = querySnapshot.Documents.ToList().FirstOrDefault();
		return user?.ConvertTo<User>();
	}

	public async Task<User> GetUserByEmail(string email)
	{
		var db = GetDb();
		var citiesRef = db.Collection(UsersCollection);
		var query = citiesRef.WhereEqualTo("email", email);
		var querySnapshot = await query.GetSnapshotAsync();

		var user = querySnapshot.Documents.ToList().FirstOrDefault();
		return user?.ConvertTo<User>();
	}

	public async Task CreateUser(User user)
	{
		var db = GetDb();
		var docRef = db.Collection(UsersCollection).Document(user.Id);
		await docRef.CreateAsync(user);
	}

	public async Task PutUser(User user)
	{
		var db = GetDb();
		var docRef = db.Collection(UsersCollection).Document(user.Id);
		await docRef.SetAsync(user);
	}

	public async Task DeleteUser(string userId)
	{
		var db = GetDb();
		var docRef = db.Collection(UsersCollection).Document(userId);
		await docRef.DeleteAsync();
	}

	public async Task<Workout> GetWorkout(string workoutId)
	{
		var db = GetDb();
		var docRef = db.Collection(WorkoutsCollection).Document(workoutId);
		var snapshot = await docRef.GetSnapshotAsync();

		if (!snapshot.Exists) return null;
		var workout = snapshot.ConvertTo<Workout>();
		return workout;
	}

	public async Task CreateWorkout(Workout workout)
	{
		var db = GetDb();
		var docRef = db.Collection(WorkoutsCollection).Document(workout.Id);
		await docRef.CreateAsync(workout);
	}

	public async Task PutWorkout(Workout workout)
	{
		var db = GetDb();
		var docRef = db.Collection(WorkoutsCollection).Document(workout.Id);
		await docRef.SetAsync(workout);
	}

	public async Task DeleteWorkout(string workoutId)
	{
		var db = GetDb();
		var docRef = db.Collection(WorkoutsCollection).Document(workoutId);
		await docRef.DeleteAsync();
	}

	public async Task<SharedWorkout> GetSharedWorkout(string sharedWorkoutId)
	{
		var db = GetDb();
		var docRef = db.Collection(SharedWorkoutCollection).Document(sharedWorkoutId);
		var snapshot = await docRef.GetSnapshotAsync();

		if (!snapshot.Exists) return null;
		var sharedWorkout = snapshot.ConvertTo<SharedWorkout>();
		return sharedWorkout;
	}

	public async Task DeleteSharedWorkout(string workoutId)
	{
		var db = GetDb();
		var docRef = db.Collection(SharedWorkoutCollection).Document(workoutId);
		await docRef.DeleteAsync();
	}
}