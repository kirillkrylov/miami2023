namespace DemoAngularBackEnd
{
	using AutoMapper;
	using Dto;
	using ErrorOr;
	using FluentValidation;
	using MediatR;
	using Microsoft.Extensions.DependencyInjection;
	using Services;
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Terrasoft.Core.Factories;

	public interface IDemoAngularBackEnd
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

	[DefaultBinding(typeof(IDemoAngularBackEnd), Name = "DemoAngularBackEnd")]
	public class DemoAngularBackEnd : IDemoAngularBackEnd
	{

		#region Constants: Private

		private const string GithubBaseUrl = "https://api.github.com/";

		#endregion

		#region Fields: Private

		private readonly IServiceProvider _serviceProvider;

		#endregion

		#region Constructors: Public

		public DemoAngularBackEnd() {
			ServiceCollection services = new ServiceCollection();
			services.AddMediatR(typeof(DemoAngularBackEnd));
			services.AddAutoMapper(typeof(DemoAngularBackEnd));
			services.AddValidatorsFromAssemblyContaining(typeof(DemoAngularBackEnd));
			services
				.AddHttpClient("GitHub", cfg => {
					cfg.BaseAddress = new Uri(GithubBaseUrl);
					cfg.DefaultRequestHeaders.Add("User-Agent", "CreatioDemoAngularBackEnd/7.8.0");
					cfg.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
				});

			services.AddTransient<IGithubService, GitHubService>();

			_serviceProvider = services
				.BuildServiceProvider(true)
				.CreateScope()
				.ServiceProvider;
		}

		#endregion

		#region Methods: Public

		public Task<ErrorOr<List<User>>> FindUser(string searchValue) {
			IGithubService githubService = _serviceProvider.GetRequiredService<IGithubService>();
			return githubService.FindUser(searchValue);
		}

		public Task<ErrorOr<List<Week>>> GetCalendarForUser(string login) {
			IGithubService githubService = _serviceProvider.GetRequiredService<IGithubService>();
			return githubService.GetCalendarForUser(login);
		}
		public Task<int[]> GetCalendarDaysDisplay(string login) {
			IGithubService githubService = _serviceProvider.GetRequiredService<IGithubService>();
			return githubService.GetCalendarDaysDisplay(login);
		}

		#endregion

	}
}