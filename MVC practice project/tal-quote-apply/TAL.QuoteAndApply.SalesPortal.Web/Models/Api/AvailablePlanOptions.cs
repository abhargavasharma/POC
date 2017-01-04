using System.Collections.Generic;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class AvailablePlanOptions
    {
        public string PlanCode { get; set; }
        public IEnumerable<string> AvailablePlans { get; set; }
        public IEnumerable<string> AvailableCovers { get; set; }

        public IEnumerable<AvailableRiderOptions> AvailableRiders { get; set; }

        public IEnumerable<string> AvailableOptions { get; set; }
    }
}