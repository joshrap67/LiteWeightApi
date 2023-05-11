using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.CurrentUser.Requests;

public class UpdateIconRequest
{
	/// <summary>
	/// Binary data of the image to upload.
	/// </summary>
	/// <example>[0010001011010101000011010]</example> // todo
	[Required]
	public byte[] ImageData { get; set; }
}