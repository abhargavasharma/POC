using ACMEFlightsAPI.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace ACMEFlightsAPI.Service
{
	public interface IFlightService
	{
		Task<bool> GetFlightAvailabilityAsync(FlightAvailabilitySearchCriteria searchCriteria);
	}

	public class FlightService : IFlightService
	{
		public async Task<bool> GetFlightAvailabilityAsync(FlightAvailabilitySearchCriteria searchCriteria)
		{
			var resp = Utilities.Utility.CreateManualResponseFromObject(true, HttpStatusCode.OK);
			return await resp.Content.ReadAsAsync<bool>();
		}
	}
}