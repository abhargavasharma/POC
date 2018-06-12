using Microsoft.Owin.Security.OAuth;
using ACMEFlightsAPI.provider;
using Owin;
using System;
using System.Configuration;

namespace ACMEFlightsAPI
{
	public partial class Startup
	{
		public static  OAuthAuthorizationServerOptions OAuthOption { get; set; }

		static Startup()
		{
            var accessTokenExpireTimeSpan = Convert.ToInt32(ConfigurationManager.AppSettings["AccessTokenExpireTimeSpan"]);
            //http://localhost:64352/token , Content-Type=application/x-www-form-urlencoded, grant_type = password and post request with username, password parameters
            OAuthOption = new OAuthAuthorizationServerOptions()
			{
				AllowInsecureHttp = true,
				TokenEndpointPath = new Microsoft.Owin.PathString("/token"),
				AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(accessTokenExpireTimeSpan),
				Provider = new OAuthAppProvider()
			};
		}

		public void ConfigureAuth(IAppBuilder app)
		{
			app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

			app.UseOAuthAuthorizationServer(OAuthOption);
			app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
		}
	}
}