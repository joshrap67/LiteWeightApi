using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.Self.Requests;

public class UpdateProfilePictureRequest
{
	/// <summary>
	/// Base 64 encoding of the image to upload.
	/// </summary>
	/// <example>iVBORw0KGgoAAAANSUhEUgAAAlgAAAJ</example>
	[Required]
	public byte[] ImageData { get; set; }
}