using Serilog;
using System.Web.Http;

namespace OAuthTokenBasedRestService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
			UnityConfig.RegisterComponents();
			GlobalConfiguration.Configure(WebApiConfig.Register);
		}

		protected void Application_Error()
		{
			var exception = Server.GetLastError();
			Server.ClearError();

			//Log exception here
			Log.Error(exception.InnerException.ToString() + ", stack trace:"+ exception.StackTrace);
		}
	}
}
