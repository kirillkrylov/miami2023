namespace DemoAngularBackEnd.Dto
{
	using Newtonsoft.Json;

	[JsonObject]
	public class User
	{

		/// <summary>
		/// URL of the account's avatar.
		/// </summary>
		[JsonProperty("avatar_url")]
		public string AvatarUrl { get; set; }


		/// <summary>
		/// The user's login.
		/// </summary>
		[JsonProperty("login")]
		public string Login { get; set; }


		/// <summary>
		/// The account's system-wide unique Id.
		/// </summary>
		[JsonProperty("id")]
		public int Id { get; set; }


		/// <summary>
		/// The user's API URL.
		/// </summary>
		[JsonProperty("url")]
		public string Url { get; set; }


		/// <summary>
		/// The HTML URL for the account on github.com (or GitHub Enterprise).
		/// </summary>
		[JsonProperty("html_url")]
		public string HtmlUrl { get; set; }
	}
}