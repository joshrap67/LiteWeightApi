using Google.Cloud.Storage.V1;

namespace LiteWeightApi.Services;

public interface IStorageService
{
	Task UploadProfilePicture(byte[] fileData, string fileName);
	Task DeleteProfilePicture(string fileName);
	Task<string> UploadDefaultProfilePicture();
}

public class StorageService : IStorageService
{
	private const string ProfilePictureBucket = "liteweight-profile-pictures";
	private const string DefaultProfilePictureBucket = "liteweight-private-images";
	private const string DefaultProfilePictureObject = "DefaultProfilePicture.jpg";

	public async Task UploadProfilePicture(byte[] fileData, string fileName)
	{
		using var stream = new MemoryStream(fileData);
		var storage = await StorageClient.CreateAsync();

		await storage.UploadObjectAsync(ProfilePictureBucket, fileName, "image/jpeg", stream);
	}

	public async Task DeleteProfilePicture(string fileName)
	{
		var storage = await StorageClient.CreateAsync();
		await storage.DeleteObjectAsync(ProfilePictureBucket, fileName);
	}

	public async Task<string> UploadDefaultProfilePicture()
	{
		var fileName = Guid.NewGuid().ToString();
		var storage = await StorageClient.CreateAsync();

		var memoryStream = new MemoryStream();
		await storage.DownloadObjectAsync(DefaultProfilePictureBucket, DefaultProfilePictureObject, memoryStream);
		var bytes = memoryStream.ToArray();

		await UploadProfilePicture(bytes, fileName);

		return fileName;
	}
}