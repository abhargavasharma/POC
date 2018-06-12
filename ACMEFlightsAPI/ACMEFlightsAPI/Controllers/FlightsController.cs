using ACMEFlightsAPI.DependencyInjection;
using ACMEFlightsAPI.Service;
using Serilog;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ACMEFlightsAPI.Controllers
{
    [Authorize]
    public class FlightsController : ApiController
    {
        private readonly IUserService _userService;

        public FlightsController()
        {
            _userService = ObjectFactory.Resolve<IUserService>();
        }

        /// <summary>
        /// Get the availability of the flight based on input criteria
        /// </summary>
        /// <param name="startDate">From date</param>
        /// <param name="endDate">To date</param>
        /// <param name="noOfPax">Number of passengers to board</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Flights/CheckFlightAvailability/{startDate}/{endDate}/{noOfPax}")]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.OK, null, typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> CheckFlightAvailability(DateTime startDate, DateTime endDate, int noOfPax)
        {
            try
            {
                //var user = await _userService.GetUserByCredentialsAsync(username, password);

                //return Ok("Hello " + user.Name + ", Your role is:" + user.Role);
                return Ok("Done");
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, $"Input: Start Date: {startDate}, End Date: {endDate}, No. Of Pax: {noOfPax}");
                return BadRequest("Provided details are invalid");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Input: Start Date: {startDate}, End Date: {endDate}, No. Of Pax: {noOfPax}");
                return InternalServerError();
            }
        }
    }
}