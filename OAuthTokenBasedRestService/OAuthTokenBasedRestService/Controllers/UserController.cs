using OAuthTokenBasedRestService.DependencyInjection;
using OAuthTokenBasedRestService.Service;
using Serilog;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace OAuthTokenBasedRestService.Services
{
	public class UserController : ApiController
	{
		private readonly IUserService _userService;

		public UserController()
		{
			_userService = ObjectFactory.Resolve<IUserService>();
		}

		[AllowAnonymous]
		[HttpGet]
		[SwaggerResponseRemoveDefaults]
		[Route("api/user/forall")]
		public IHttpActionResult welcome()
		{
			Log.Error(new System.Exception("Testing the Serolog file configuration 111"), "");

			return Ok("Welcome to UserController");
		}

		[Authorize]
		[HttpGet]
		[SwaggerResponseRemoveDefaults]
		[Route("api/user/getusers")]
		public IHttpActionResult GetUsers()
		{
			var identity = (ClaimsIdentity)User.Identity;

			return Ok("Hello " + identity.Name + ", Your authentication type :" + identity.IsAuthenticated);
		}

		/// <summary>
		/// Getting the user details using Authorization Oauth Token(bearer + OAuth token)
		/// http://localhost:64352/api/user/getuserdetails/bhargav@test.com/123456
		/// Swagger Url: http://localhost:64352/swagger
		/// </summary>
		/// <param name="email"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		[Authorize]
		[HttpGet]
		[Route("api/user/getuserdetails/{email}/{password}")]
		[SwaggerResponseRemoveDefaults]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.OK, null, typeof(string))]
		public async Task<IHttpActionResult> GetUserDetails(string email, string password)
		{
			try
			{
				var user = await _userService.GetUserByCredentialsAsync(email, password);

				return Ok("Hello " + user.Name + ", Your role is:" + user.Role);
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Input: User name: {email}, Password {password}");
				return BadRequest("Provided email and password are invalid");
			}
			
		}

	}
}