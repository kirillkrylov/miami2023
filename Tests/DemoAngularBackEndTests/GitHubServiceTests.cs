using NUnit.Framework;

namespace DemoAngularBackEndTests
{
	using DemoAngularBackEnd.Services;
	using Microsoft.Extensions.DependencyInjection;
	using System;
	using System.IO;
	using System.Net;
	using System.Net.Http;
	using System.Threading;
	using System.Threading.Tasks;

	[TestFixture(Author = "Kirill Krylov")]
	public class GitHubServiceTests
	{

		private GitHubService _sut;
		private const string _githubBaseUrl = "https://api.github.com/";

		[SetUp]
		public void Setup() {
			IServiceCollection services = new ServiceCollection();
			services.AddHttpClient("GitHub", cfg => {
				cfg.BaseAddress = new Uri(_githubBaseUrl);
				cfg.DefaultRequestHeaders.Add("User-Agent", "CreatioDemoAngularBackEnd/7.8.0");
				cfg.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
			})
				.ConfigurePrimaryHttpMessageHandler(() => new CallBackHttpClientMessageHandler());
			IServiceProvider serviceProvider = services.BuildServiceProvider();
			IHttpClientFactory httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

			_sut = new GitHubService(httpClientFactory);
		}

		[Category("HttpClient")]
		[Test(Author = "Kirill Krylov")]
		public void ShouldReturn_WhenCalled_WithValidParams() {

			Assert.Multiple(async () => {

				var actual = await _sut.FindUser("k.krylov@creatio.com");

				Assert.That(actual.IsError, Is.False);
				Assert.That(actual.Value.Count, Is.EqualTo(1));
				Assert.That(actual.Value[0].Login, Is.EqualTo("kirillkrylov"));
			});
		}
	}


	internal class CallBackHttpClientMessageHandler : DelegatingHandler
	{

		#region Methods: Protected

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
			CancellationToken cancellationToken) {

			var dllLocation = GetType().Assembly.Location;
			FileInfo fi = new FileInfo(dllLocation);
			var dir = fi.Directory;
			var jsonPath = Path.Combine(dir.FullName, "MockResponse", "Mock_FindUser_kirillkrylov.json");
			var jsonFile = File.ReadAllText(jsonPath);


			Assert.Multiple(async () => {
				Assert.That(request.RequestUri.AbsoluteUri, Is.EqualTo($"https://api.github.com/search/users?q=k.krylov@creatio.com+type:user"));
				Assert.That(request.Method, Is.EqualTo(HttpMethod.Get));
			});


			HttpResponseMessage mockResponse = new HttpResponseMessage() {
				Content = new StringContent(jsonFile),
				StatusCode = HttpStatusCode.OK
			};
			return mockResponse;
		}

		#endregion
	}
}