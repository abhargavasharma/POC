using Owin;
using System.Web.Http;
using Serilog;
using Serilog.Exceptions;

namespace ACMEFlightsAPI
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
			Log.Logger = new LoggerConfiguration()
				.Enrich.WithExceptionDetails()
			.ReadFrom.AppSettings()
			.CreateLogger();
			HttpConfiguration config = new HttpConfiguration();
			WebApiConfig.Register(config);
		}

	}
}