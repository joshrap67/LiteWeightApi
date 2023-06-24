using Google.Cloud.Firestore;
using LiteWeightAPI.Api.Complaints.Responses;
using LiteWeightAPI.Domain.Complaints;
using LiteWeightAPI.Domain.SharedWorkouts;
using LiteWeightAPI.Domain.Users;
using LiteWeightAPI.Domain.Workouts;
using LiteWeightAPI.Options;
using Microsoft.Extensions.Options;

namespace LiteWeightAPI.Domain;

public interface IRepository
{
	Task<User> GetUser(string userId);
	Task<User> GetUserByUsername(string username);
	Task<User> GetUserByEmail(string email);
	Task CreateUser(User user);
	Task PutUser(User user);
	Task DeleteUser(string userId);
	Task CreateComplaint(Complaint complaint);
	Task<Workout> GetWorkout(string workoutId);
	Task CreateWorkout(Workout workout);
	Task PutWorkout(Workout workout);
	Task DeleteWorkout(string workoutId);
	Task<SharedWorkout> GetSharedWorkout(string sharedWorkoutId);
	Task DeleteSharedWorkout(string workoutId);
	Task<Complaint> GetComplaint(string complaintId);

	Task ExecuteBatchWrite(IList<Workout> workoutsToPut = null, IList<User> usersToPut = null,
		IList<SharedWorkout> sharedWorkoutsToPut = null, IList<Workout> workoutsToDelete = null,
		IList<User> usersToDelete = null, IList<SharedWorkout> sharedWorkoutsToDelete = null);
}

public class Repository : IRepository
{
	private readonly FirestoreOptions _fireStoreOptions;
	private readonly FirebaseOptions _firebaseOptions;

	public Repository(IOptions<FirestoreOptions> fireStoreOptions, IOptions<FirebaseOptions> firebaseOptions)
	{
		_fireStoreOptions = fireStoreOptions.Value;
		_firebaseOptions = firebaseOptions.Value;
	}

	private FirestoreDb GetDb()
	{
		return FirestoreDb.Create(_firebaseOptions.ProjectId);
	}

	public async Task<Complaint> GetComplaint(string complaintId)
	{
		var db = GetDb();
		var docRef = db.Collection(_fireStoreOptions.ComplaintsCollection).Document(complaintId);
		var snapshot = await docRef.GetSnapshotAsync();

		if (!snapshot.Exists) return null;
		var complaint = snapshot.ConvertTo<Complaint>();
		return complaint;
	}

	public async Task ExecuteBatchWrite(IList<Workout> workoutsToPut = null, IList<User> usersToPut = null,
		IList<SharedWorkout> sharedWorkoutsToPut = null, IList<Workout> workoutsToDelete = null,
		IList<User> usersToDelete = null, IList<SharedWorkout> sharedWorkoutsToDelete = null)
	{
		var db = GetDb();
		var batch = db.StartBatch();

		foreach (var workout in workoutsToDelete ?? new List<Workout>())
		{
			var workoutsRef = db.Collection(_fireStoreOptions.WorkoutsCollection).Document(workout.Id);
			batch.Delete(workoutsRef);
		}

		foreach (var workout in workoutsToPut ?? new List<Workout>())
		{
			var workoutsRef = db.Collection(_fireStoreOptions.WorkoutsCollection).Document(workout.Id);
			batch.Set(workoutsRef, workout);
		}

		foreach (var user in usersToDelete ?? new List<User>())
		{
			var usersRef = db.Collection(_fireStoreOptions.UsersCollection).Document(user.Id);
			batch.Delete(usersRef);
		}

		foreach (var user in usersToPut ?? new List<User>())
		{
			var usersRef = db.Collection(_fireStoreOptions.UsersCollection).Document(user.Id);
			batch.Set(usersRef, user);
		}

		foreach (var sharedWorkout in sharedWorkoutsToDelete ?? new List<SharedWorkout>())
		{
			var sharedWorkoutRef = db.Collection(_fireStoreOptions.SharedWorkoutsCollection).Document(sharedWorkout.Id);
			batch.Delete(sharedWorkoutRef);
		}

		foreach (var sharedWorkout in sharedWorkoutsToPut ?? new List<SharedWorkout>())
		{
			var sharedWorkoutRef = db.Collection(_fireStoreOptions.SharedWorkoutsCollection).Document(sharedWorkout.Id);
			batch.Set(sharedWorkoutRef, sharedWorkout);
		}

		await batch.CommitAsync();
	}

	public async Task<User> GetUser(string userId)
	{
		var db = GetDb();
		var docRef = db.Collection(_fireStoreOptions.UsersCollection).Document(userId);
		var snapshot = await docRef.GetSnapshotAsync();

		if (!snapshot.Exists) return null;
		var user = snapshot.ConvertTo<User>();
		return user;
	}

	public async Task<User> GetUserByUsername(string username)
	{
		var db = GetDb();
		var usersRef = db.Collection(_fireStoreOptions.UsersCollection);
		var query = usersRef.WhereEqualTo("username", username.ToLowerInvariant());
		var querySnapshot = await query.GetSnapshotAsync();

		var user = querySnapshot.Documents.ToList().FirstOrDefault();
		return user?.ConvertTo<User>();
	}

	public async Task<User> GetUserByEmail(string email)
	{
		var db = GetDb();
		var usersRef = db.Collection(_fireStoreOptions.UsersCollection);
		var query = usersRef.WhereEqualTo("email", email);
		var querySnapshot = await query.GetSnapshotAsync();

		var user = querySnapshot.Documents.ToList().FirstOrDefault();
		return user?.ConvertTo<User>();
	}

	public async Task CreateUser(User user)
	{
		var db = GetDb();
		var docRef = db.Collection(_fireStoreOptions.UsersCollection).Document(user.Id);
		await docRef.CreateAsync(user);
	}

	public async Task PutUser(User user)
	{
		var db = GetDb();
		var docRef = db.Collection(_fireStoreOptions.UsersCollection).Document(user.Id);
		await docRef.SetAsync(user);
	}

	public async Task DeleteUser(string userId)
	{
		var db = GetDb();
		var docRef = db.Collection(_fireStoreOptions.UsersCollection).Document(userId);
		await docRef.DeleteAsync();
	}

	public async Task CreateComplaint(Complaint complaint)
	{
		var db = GetDb();
		var docRef = db.Collection(_fireStoreOptions.ComplaintsCollection).Document(complaint.Id);
		await docRef.CreateAsync(complaint);
	}

	public async Task<Workout> GetWorkout(string workoutId)
	{
		var db = GetDb();
		var docRef = db.Collection(_fireStoreOptions.WorkoutsCollection).Document(workoutId);
		var snapshot = await docRef.GetSnapshotAsync();

		if (!snapshot.Exists) return null;
		var workout = snapshot.ConvertTo<Workout>();
		return workout;
	}

	public async Task CreateWorkout(Workout workout)
	{
		var db = GetDb();
		var docRef = db.Collection(_fireStoreOptions.WorkoutsCollection).Document(workout.Id);
		await docRef.CreateAsync(workout);
	}

	public async Task PutWorkout(Workout workout)
	{
		var db = GetDb();
		var docRef = db.Collection(_fireStoreOptions.WorkoutsCollection).Document(workout.Id);
		await docRef.SetAsync(workout);
	}

	public async Task DeleteWorkout(string workoutId)
	{
		var db = GetDb();
		var docRef = db.Collection(_fireStoreOptions.WorkoutsCollection).Document(workoutId);
		await docRef.DeleteAsync();
	}

	public async Task<SharedWorkout> GetSharedWorkout(string sharedWorkoutId)
	{
		var db = GetDb();
		var docRef = db.Collection(_fireStoreOptions.SharedWorkoutsCollection).Document(sharedWorkoutId);
		var snapshot = await docRef.GetSnapshotAsync();

		if (!snapshot.Exists) return null;
		var sharedWorkout = snapshot.ConvertTo<SharedWorkout>();
		return sharedWorkout;
	}

	public async Task DeleteSharedWorkout(string workoutId)
	{
		var db = GetDb();
		var docRef = db.Collection(_fireStoreOptions.SharedWorkoutsCollection).Document(workoutId);
		await docRef.DeleteAsync();
	}
}