using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACMEFlightsAPI.Models
{
	public class FlightAvailabilitySearchCriteria
	{
		/// <summary>
		/// From date
		/// </summary>
		[Required]
		public DateTime FromDate { get; set; }

		/// <summary>
		/// To date
		/// </summary>
		[Required]
		public DateTime ToDate { get; set; }

		/// <summary>
		/// Number of passengers
		/// </summary>
		[Required]
		public int? NoOfPax { get; set; }
	}
}