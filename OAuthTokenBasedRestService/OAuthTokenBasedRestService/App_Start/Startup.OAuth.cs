using Microsoft.Owin.Security.OAuth;
using OAuthTokenBasedRestService.provider;
using Owin;
using System;

namespace OAuthTokenBasedRestService
{
	public partial class Startup
	{
		public static  OAuthAuthorizationServerOptions OAuthOption { get; set; }

		static Startup()
		{
			//http://localhost:64352/token , Content-Type=application/x-www-form-urlencoded, grant_type = password and post request with username, password parameters
			OAuthOption = new OAuthAuthorizationServerOptions()
			{
				AllowInsecureHttp = true,
				TokenEndpointPath = new Microsoft.Owin.PathString("/token"),
				AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(480),//480 Can be configured
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