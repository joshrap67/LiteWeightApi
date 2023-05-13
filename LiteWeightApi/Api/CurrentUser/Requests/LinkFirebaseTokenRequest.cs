using System.ComponentModel.DataAnnotations;

namespace LiteWeightApi.Api.CurrentUser.Requests;

public class LinkFirebaseTokenRequest
{
	/// <summary>
	/// Firebase token to associate with the current user. Allows for push notifications to be sent to the device associated with the token.
	/// </summary>
	/// <example>c_-_jNKuQI-9USNk0c9uEY:APA91bG53pjv4wNcg7m2h2d1yfCjVfidEj7AXyIu8ddNYz2_Stwy0J7znQayzltXDgEL8Q9tSj8i2yx8hQiSuNDKlZtGm3cEmJTlLMupmJVTc1g_LdMPdx9VOL2hC3vBTseIcmBDYjRG</example>
	[Required]
	public string FirebaseToken { get; set; }
}