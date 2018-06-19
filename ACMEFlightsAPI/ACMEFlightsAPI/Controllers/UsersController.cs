using ACMEFlightsAPI.DependencyInjection;
using ACMEFlightsAPI.Service;
using Serilog;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ACMEFlightsAPI.Services
{
    [Authorize]
    public class UsersController : ApiController
	{
		private readonly IUserService _userService;

		public UsersController()
		{
			_userService = ObjectFactory.Resolve<IUserService>();
		}

        /// <summary>
        /// Getting the user details using Authorization Oauth Token (bearer + OAuth token) 
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="password">Password to access tho solution</param>
        /// <returns></returns>
        [HttpGet]
		[SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.OK, null, typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetUserDetails(string username, string password)
		{
			try
			{
				var user = await _userService.GetUserByCredentialsAsync(username, password);

				return Ok("Hello " + user.Name + ", Your role is:" + user.Role);
			}
            catch (HttpRequestException ex)
            {
                Log.Error(ex, $"Input: User name: {username}, Password {password}");
                return BadRequest("Provided email and password are invalid");
            }
            catch (Exception ex)
			{
				Log.Error(ex, $"Input: User name: {username}, Password {password}");
                return InternalServerError();
			}
		}
	}
}