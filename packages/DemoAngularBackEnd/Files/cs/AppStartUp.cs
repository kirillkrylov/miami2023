namespace DemoAngularBackEnd
{
	using Terrasoft.Core.Factories;
	using Terrasoft.Web.Common;


	public class AppStartUp : IAppEventListener
	{

		public static IDemoAngularBackEnd DemoAngularBackEnd { get; private set; }

		public void OnAppStart(AppEventContext context) {
			DemoAngularBackEnd = ClassFactory.Get<IDemoAngularBackEnd>("DemoAngularBackEnd");
		}

		public void OnAppEnd(AppEventContext context) {
		}

		public void OnSessionStart(AppEventContext context) {
		}

		public void OnSessionEnd(AppEventContext context) {
		}

	}
}
