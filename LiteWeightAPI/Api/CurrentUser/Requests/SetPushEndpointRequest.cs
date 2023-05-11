using System.ComponentModel.DataAnnotations;

namespace LiteWeightAPI.Api.CurrentUser.Requests;

public class SetPushEndpointRequest
{
	/// <summary>
	/// Firebase token that identifies the user's current device.
	/// </summary>
	/// <example>c_-_jNKuQI-9USNk0c9uEY:APA91bG53pjv4wNcg7m2h2d1yfCjVfidEj7AXyIu8ddNYz2_Stwy0J7znQayzltXDgEL8Q9tSj8i2yx8hQiSuNDKlZtGm3cEmJTlLMupmJVTc1g_LdMPdx9VOL2hC3vBTseIcmBDYjRG</example>
	[Required]
	public string FirebaseToken { get; set; }
}