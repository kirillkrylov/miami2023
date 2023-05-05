namespace DemoAngularBackEnd.Services
{
	using Dto;
	using ErrorOr;
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Threading.Tasks;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Terrasoft.Core.Configuration;
	using Terrasoft.Core.Factories;

	internal interface IGithubService
	{

		#region Methods: Public

		/// <summary>
		/// Finds the user on GitHub.
		/// </summary>
		/// <param name="searchValue">The value to search
		/// <remarks>Can be username or email</remarks>
		/// </param>
		Task<ErrorOr<List<User>>> FindUser(string searchValue);

		/// <summary>
		/// Gets the GitHub calendar for login.
		/// </summary>
		/// <param name="login">The login.
		/// <remarks>Use <see cref="FindUser"/> to find user
		/// </remarks>
		/// </param>
		/// <returns></returns>
		Task<ErrorOr<List<Week>>> GetCalendarForUser(string login);

		#endregion

	}

	internal sealed class GitHubService : IGithubService, IDisposable
	{

		#region Fields: Private

		private static readonly Func<string, string> GetGraphQlRequestForCalendarSearch =
			login =>
				$"{{\"query\":\"query($start: DateTime, $end: DateTime){{user(login: \\\"{login}\\\") {{email, createdAt,contributionsCollection(from: $start,to: $end){{contributionCalendar {{weeks {{contributionDays {{weekday date contributionCount color}}}}}}}}}}}}\",\"variables\":{{\"start\":\"{DateTimeOffset.UtcNow.AddYears(-1):yyyy-MM-ddThh:mm:ssZ}\",\"end\":\"{DateTimeOffset.UtcNow:yyyy-MM-ddThh:mm:ssZ}\"}}}}";

		private static readonly Func<string> GetCurrentUserToken = () => {

			return SysSettings.GetValue(
				ClassFactory.Get<Terrasoft.Core.UserConnection>(),
				"GitHubUserToken",
				string.Empty);
		};

		private readonly HttpClient _httpClient;

		#endregion

		#region Constructors: Public

		public GitHubService(IHttpClientFactory httpClientFactory) {
			_httpClient = httpClientFactory.CreateClient("GitHub");
		}

		#endregion

		#region Methods: Public

		public void Dispose() {
			_httpClient.Dispose();
		}

		public async Task<ErrorOr<List<User>>> FindUser(string searchValue) {
			try {
				string query = $"search/users?q={searchValue}+type:user";
				HttpResponseMessage response = await _httpClient.GetAsync(query);
				response.EnsureSuccessStatusCode();
				string stringContent = await response.Content.ReadAsStringAsync();
				SearchResult searchResult = JsonConvert.DeserializeObject<SearchResult>(stringContent);
				return searchResult.Items;
			} catch(Exception e) {
				return Error.Failure(e.Message);
			}
		}

		public async Task<ErrorOr<List<Week>>> GetCalendarForUser(string login) {
			_httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("bearer", GetCurrentUserToken());
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "graphql") {
				Content = new StringContent(
					GetGraphQlRequestForCalendarSearch(login),
					Encoding.UTF8) {
					Headers = {
						ContentType = new MediaTypeHeaderValue("application/json")
					}
				}
			};
			HttpResponseMessage response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();
			try {
				string responseString = await response.Content.ReadAsStringAsync();
				JObject jObject = JObject.Parse(responseString);
				const string jsonPath = "$.data.user.contributionsCollection.contributionCalendar.weeks";
				JToken token = jObject.SelectToken(jsonPath);
				List<Week> weeks = token.ToObject<List<Week>>();
				return weeks;
			} catch(Exception e) {
				return Error.Failure(e.Message);
			}
		}

		#endregion

	}
}