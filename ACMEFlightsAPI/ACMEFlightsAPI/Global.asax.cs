using ACMEFlightsAPI.Providers;
using Serilog;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace ACMEFlightsAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
			UnityConfig.RegisterComponents();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), new ApiVersioningSelector(GlobalConfiguration.Configuration));
		}

		protected void Application_Error()
		{
			var exception = Server.GetLastError();
			Server.ClearError();

			//Serilog logging exception here
			Log.Error(exception.InnerException.ToString() + ", stack trace:"+ exception.StackTrace);
		}
	}
}
