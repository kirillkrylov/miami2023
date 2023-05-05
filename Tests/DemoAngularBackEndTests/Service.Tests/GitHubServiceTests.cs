using NUnit.Framework;

namespace DemoAngularBackEndTests
{
	using DemoAngularBackEnd.Services;
	using Microsoft.Extensions.DependencyInjection;
	using NSubstitute;
	using System;
	using System.IO;
	using System.Net;
	using System.Net.Http;
	using System.Threading;
	using System.Threading.Tasks;
	using Terrasoft.Configuration.Tests;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.TestFramework;
	using Terrasoft.UnitTest;

	[TestFixture(Author = "Kirill Krylov")]
	[MockSettings(RequireMock.All)]
	public class GitHubServiceTests : BaseConfigurationTestFixture
	{

		private GitHubService _sut;
		private const string _githubBaseUrl = "https://api.github.com/";
		private CallBackHttpClientMessageHandler CallBackHttpClientMessageHandler => new CallBackHttpClientMessageHandler();
		private IServiceCollection services;


		public GitHubServiceTests() {
			services = new ServiceCollection();
		}

		
		[Category("HttpClient")]
		[Test(Author = "Kirill Krylov")]
		public void ShouldReturn_WhenCalled_WithValidParams() {
			//Arrange
			CallBackHttpClientMessageHandler callBackHttpClientMessageHandler = new CallBackHttpClientMessageHandler();
			callBackHttpClientMessageHandler.Configure("Mock_FindUser_kirillkrylov.json", HttpStatusCode.OK);
			services.AddHttpClient("GitHub", cfg => {
				cfg.BaseAddress = new Uri(_githubBaseUrl);
				cfg.DefaultRequestHeaders.Add("User-Agent", "CreatioDemoAngularBackEnd/7.8.0");
				cfg.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
			})
				.ConfigurePrimaryHttpMessageHandler(() => callBackHttpClientMessageHandler);

			IServiceProvider serviceProvider = services.BuildServiceProvider();
			IHttpClientFactory httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
			_sut = new GitHubService(httpClientFactory);


			//Act and Assert
			Assert.Multiple(async () => {

				var actual = await _sut.FindUser("k.krylov@creatio.com");

				Assert.That(actual.IsError, Is.False);
				Assert.That(actual.Value.Count, Is.EqualTo(1));
				Assert.That(actual.Value[0].Login, Is.EqualTo("kirillkrylov"));
			});
		}


		[Category("HttpClient")]
		[Test(Author = "Kirill Krylov")]
		public void ShouldReturnEmptyCollection_WhenCalled_WithValidParams() {
			//Arrange
			CallBackHttpClientMessageHandler callBackHttpClientMessageHandler = new CallBackHttpClientMessageHandler();
			callBackHttpClientMessageHandler.Configure("Mock_FindUser_Empty.json", HttpStatusCode.OK);
			services.AddHttpClient("GitHub", cfg => {
				cfg.BaseAddress = new Uri(_githubBaseUrl);
				cfg.DefaultRequestHeaders.Add("User-Agent", "CreatioDemoAngularBackEnd/7.8.0");
				cfg.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
			})
				.ConfigurePrimaryHttpMessageHandler(() => callBackHttpClientMessageHandler);

			IServiceProvider serviceProvider = services.BuildServiceProvider();
			IHttpClientFactory httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
			_sut = new GitHubService(httpClientFactory);


			//Act and Assert
			Assert.Multiple(async () => {

				var actual = await _sut.FindUser("");

				Assert.That(actual.IsError, Is.False);
				Assert.That(actual.Value.Count, Is.EqualTo(0));
			});
		}


		[Category("HttpClient")]
		[Test(Author = "Kirill Krylov")]
		public void ShouldReturnCalendar_WhenCalled_WithValidParams() {
			//Arrange

			UserConnection.SettingsValues.Add("GitHubUserToken", "myToken_here");
			
			ClassFactory.RebindWithFactoryMethod(() => {
			
					return (UserConnection)UserConnection;
			});

			CallBackHttpClientMessageHandler callBackHttpClientMessageHandler = new CallBackHttpClientMessageHandler();
			callBackHttpClientMessageHandler.Configure("Mock_Calendar.json", HttpStatusCode.OK);
			services.AddHttpClient("GitHub", cfg => {
				cfg.BaseAddress = new Uri(_githubBaseUrl);
				cfg.DefaultRequestHeaders.Add("User-Agent", "CreatioDemoAngularBackEnd/7.8.0");
				cfg.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
			})
				.ConfigurePrimaryHttpMessageHandler(() => callBackHttpClientMessageHandler);

			IServiceProvider serviceProvider = services.BuildServiceProvider();
			IHttpClientFactory httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
			_sut = new GitHubService(httpClientFactory);


			//Act and Assert
			Assert.Multiple(async () => {

				var actual = await _sut.GetCalendarForUser("kirillkrylov");
				Assert.That(actual.IsError, Is.False);
				Assert.That(actual.Value.Count, Is.EqualTo(53));
			});
		}

	}


	internal class CallBackHttpClientMessageHandler : DelegatingHandler
	{
		private string _jsonPath = string.Empty;
		private HttpStatusCode _httpStatusCode;

		public void Configure(string fileName, HttpStatusCode httpStatusCode) {

			string dllLocation = GetType().Assembly.Location;
			_jsonPath = Path.Combine(new FileInfo(dllLocation).Directory.FullName, "MockResponse", fileName);
			_httpStatusCode = httpStatusCode;
		}

		#region Methods: Protected

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
			CancellationToken cancellationToken) {
			HttpResponseMessage mockResponse = new HttpResponseMessage() {
				Content = new StringContent(File.ReadAllText(_jsonPath)),
				StatusCode = _httpStatusCode
			};
			return mockResponse;
		}

		#endregion
	}
}