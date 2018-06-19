using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ACMEFlightsAPI.Attributes;
using ACMEFlightsAPI.DependencyInjection;
using ACMEFlightsAPI.Models;
using ACMEFlightsAPI.Service;
using Serilog;
using Swashbuckle.Swagger.Annotations;


namespace ACMEFlightsAPI.Controllers
{
	/// <summary>
	/// FlightsController
	/// </summary>
	[Attributes.Authorize]
	public class FlightsController : ApiController
	{
		private readonly IFlightService _flightService;

		/// <summary>
		/// Initializes members of FlightsController
		/// </summary>
		public FlightsController()
		{
			_flightService = ObjectFactory.Resolve<IFlightService>();
		}

		/// <summary>
		/// Get the availability of the flight based on input criteria
		/// </summary>
		/// <returns>True or fasle based on availability. True means Available and fasle meanns not available</returns>
		[CheckModelForNull]
		[ValidateModel]
		[SwaggerResponse(HttpStatusCode.OK, null, typeof(string))]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.InternalServerError)]
		public async Task<IHttpActionResult> CheckFlightAvailability([FromBody]FlightAvailabilitySearchCriteria searchCriteria)
		{
			try
			{
				if (searchCriteria.FromDate == DateTime.MinValue || searchCriteria.ToDate == DateTime.MinValue)
				{
					return BadRequest($"Provided details are invalid, From date or To date or both should not be {DateTime.MinValue}");
				}

				if (searchCriteria.NoOfPax  <= 0)
				{
					return BadRequest($"Provided details are invalid, number of passegers should be more than 0");
				}

				if (searchCriteria.FromDate < DateTime.Now || searchCriteria.ToDate < DateTime.Now)
				{
					return BadRequest($"Provided details are invalid, From date and To date should be greater than or eaqual to today");
				}

				if (searchCriteria.FromDate < searchCriteria.ToDate)
				{
					var flightAvialability = await _flightService.GetFlightAvailabilityAsync(searchCriteria);
					return Ok(flightAvialability);
				}
				else
					return BadRequest("Provided details are invalid, From date should be lower than To date");
				
			}
			catch (HttpRequestException ex)
			{
				Log.Error(ex, $"Input: Start Date: {searchCriteria.FromDate}, End Date: {searchCriteria.ToDate}, No. Of Pax: {searchCriteria.NoOfPax}");
				return BadRequest("Provided details are invalid");
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Input: Start Date: {searchCriteria.FromDate}, End Date: {searchCriteria.ToDate}, No. Of Pax: {searchCriteria.NoOfPax}");
				return InternalServerError();
			}
		}
	}
}