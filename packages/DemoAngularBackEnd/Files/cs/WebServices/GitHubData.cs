using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Terrasoft.Web.Common;

namespace DemoAngularBackEnd.WebServices
{
	using Dto;
	using ErrorOr;
	using System.Collections.Generic;
	using Terrasoft.Core.Factories;
	using Terrasoft.Core.ServiceModelContract;

	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class GitHubData : BaseService
	{

		#region Methods: Public

		//http://localhost:7080/0/rest/GitHubData/FindUser?searchValue=k.krylov@creatio.com
		/// <summary>
		/// Finds the user.
		/// </summary>
		/// <param name="searchValue">The search value.</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare,
			ResponseFormat = WebMessageFormat.Json)]
		public MyValueResponse<List<User>> FindUser(string searchValue) {

			IDemoAngularBackEnd gitHub = ClassFactory.Get<IDemoAngularBackEnd>("DemoAngularBackEnd");
			ErrorOr<List<User>> response = AppStartUp.DemoAngularBackEnd.FindUser(searchValue).Result;

			//ErrorOr<List<User>> response = gitHub.FindUser(searchValue).Result;

			if(response.IsError) {
				return new MyValueResponse<List<User>> {
					Success = false,
					ErrorInfo = new ErrorInfo {
						ErrorCode = response.FirstError.Code,
						Message = response.FirstError.Description
					}
				};
			}

			return new MyValueResponse<List<User>> {
				Success = true,
				Value = response.Value
			};
		}

		//http://localhost:7080/0/rest/GitHubData/GetCalendarForUser?login=kirillkrylov
		/// <summary>
		/// Gets the GitHub calendar for login.
		/// </summary>
		/// <param name="login">The login.
		/// <remarks>Use <see cref="FindUser"/> to find user
		/// </remarks>
		/// </param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare,
			ResponseFormat = WebMessageFormat.Json)]
		public MyValueResponse<List<Week>> GetCalendarForUser(string login) {
			ErrorOr<List<Week>> response = AppStartUp.DemoAngularBackEnd.GetCalendarForUser(login).Result;

			//IDemoAngularBackEnd gitHub = ClassFactory.Get<IDemoAngularBackEnd>("DemoAngularBackEnd");
			//ErrorOr<List<Week>> response = gitHub.GetCalendarForUser(login).Result;

			if(response.IsError) {
				return new MyValueResponse<List<Week>> {
					Success = false
				};
			}

			return new MyValueResponse<List<Week>> {
				Success = true,
				Value = response.Value
			};
		}

		#endregion

	}
}