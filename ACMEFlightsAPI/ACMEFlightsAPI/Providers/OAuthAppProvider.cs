using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using ACMEFlightsAPI.Service;
using System.Security.Claims;
using ACMEFlightsAPI.DependencyInjection;

namespace ACMEFlightsAPI.provider
{
	public class OAuthAppProvider: OAuthAuthorizationServerProvider
	{
		private readonly IUserService _userService;
		public OAuthAppProvider()
		{
			_userService = ObjectFactory.Resolve<IUserService>();
		}

		public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			context.Validated();
		}
		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			var identity = new ClaimsIdentity(context.Options.AuthenticationType);
			var username = context.UserName;
			var password = context.Password;
			var user = await _userService.GetUserByCredentialsAsync(username, password);

			if (user != null)
			{
				identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
				identity.AddClaim(new Claim("userid", user.Id));
				identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
				context.Validated(identity);
			}
			else
			{
				context.SetError("invalid_grant", "Provided email and password are incorrect");
				return;
			}
		}
	}
}