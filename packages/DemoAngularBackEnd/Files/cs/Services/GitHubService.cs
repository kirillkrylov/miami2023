namespace DemoAngularBackEnd.Services
{
	using Dto;
	using ErrorOr;
	using System;
	using System.Collections.Generic;
	using System.Linq;
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


		Task<int[]> GetCalendarDaysDisplay(string login);
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


		public async Task<int[]> GetCalendarDaysDisplay(string login) {
			var weeks = (await GetCalendarForUser(login)).Value;


			int lastDontributionDay = weeks.Last().ContributionDays.Last().WeekDay;
			DateTime now = DateTime.Now;
			int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)now.DayOfWeek + 7) % 7;


			for(var i = 0;  i < daysUntilSaturday; i++) {
				weeks.Last().ContributionDays.Add(new ContributionDay() {
					Count = 0
				});
			}

			
			int[] mondays = new int[52];
			int[] tuesdays = new int[52];
			int[] wednesdays = new int[52];
			int[] thursdays = new int[52];
			int[] fridays = new int[52];
			int[] saturdays = new int[52];
			int[] sundays = new int[52];

			int processedWeeks = 0;
			
			while(processedWeeks<52) {

				
				int i = (weeks.Count ==53)? 1: 0;

				var days = weeks[weeks.Count-1-processedWeeks];

				sundays[weeks.Count-1-processedWeeks-i] = days.ContributionDays.FirstOrDefault((d=> d.WeekDay == 0))?.Count ?? 0;
				mondays[weeks.Count-1-processedWeeks-i] = days.ContributionDays.FirstOrDefault((d=> d.WeekDay == 1))?.Count ?? 0;
				tuesdays[weeks.Count - 1 -processedWeeks-i] = days.ContributionDays.FirstOrDefault((d=> d.WeekDay == 2))?.Count ?? 0;
				wednesdays[weeks.Count - 1 -processedWeeks-i] = days.ContributionDays.FirstOrDefault((d => d.WeekDay == 3))?.Count ?? 0;
				thursdays[weeks.Count - 1 -processedWeeks-i] = days.ContributionDays.FirstOrDefault((d => d.WeekDay == 4))?.Count ?? 0;
				fridays[weeks.Count - 1 -processedWeeks-i] = days.ContributionDays.FirstOrDefault((d => d.WeekDay == 5))?.Count ?? 0;
				saturdays[weeks.Count - 1 -processedWeeks-i] = days.ContributionDays.FirstOrDefault((d=> d.WeekDay == 6))?.Count ?? 0;
				processedWeeks ++;
			}
			

			

			var result = new List<int>();
			result.AddRange(mondays);
			result.AddRange(tuesdays);
			result.AddRange(wednesdays);
			result.AddRange(thursdays);
			result.AddRange(fridays);
			result.AddRange(saturdays);
			result.AddRange(sundays);

			var max = result.Max();

			result.ForEach(i=> {
				i= (i/max);
			});
			
			return result.ToArray();

		}

		#endregion

	}
}