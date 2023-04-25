using Amazon.S3;
using Amazon.S3.Model;

namespace LiteWeightAPI.Services;

public interface IStorageService
{
	Task UploadImage(byte[] fileData, string fileName);
	Task DeleteImage(string fileName);
	Task<string> UploadDefaultImage();
}

public class StorageService : IStorageService
{
	private readonly IAmazonS3 _client;

	private const string S3ImageBucket = "liteweight-images";
	private const string S3DefaultImageBucket = "liteweight-images-private";
	private const string S3DefaultImageFile = "DefaultProfilePicture.jpg";

	public StorageService(IAmazonS3 client)
	{
		_client = client; // todo factory with credentials
	}

	public async Task UploadImage(byte[] fileData, string fileName)
	{
		using var stream = new MemoryStream(fileData);
		var request = new PutObjectRequest
		{
			BucketName = S3ImageBucket,
			Key = fileName,
			InputStream = stream,
			ContentType = "image/jpeg",
			CannedACL = S3CannedACL.PublicRead
		};
		await _client.PutObjectAsync(request);
	}

	public async Task DeleteImage(string fileName)
	{
		await _client.DeleteObjectAsync(new DeleteObjectRequest
		{
			BucketName = S3ImageBucket,
			Key = fileName
		});
	}

	public async Task<string> UploadDefaultImage()
	{
		var fileName = Guid.NewGuid().ToString();
		var downloadRequest = new GetObjectRequest
		{
			BucketName = S3DefaultImageBucket,
			Key = S3DefaultImageFile,
			ResponseHeaderOverrides = new ResponseHeaderOverrides
			{
				ContentType = "image/jpg"
			}
		};
		var defaultImageResponse = await _client.GetObjectAsync(downloadRequest);
		var memoryStream = new MemoryStream();
		await defaultImageResponse.ResponseStream.CopyToAsync(memoryStream);
		var bytes = memoryStream.ToArray();

		await UploadImage(bytes, fileName);

		return fileName;
	}
}