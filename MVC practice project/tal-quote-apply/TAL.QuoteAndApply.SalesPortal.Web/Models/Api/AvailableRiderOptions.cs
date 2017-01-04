using System.Collections.Generic;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class AvailableRiderOptions
    {
        public string RiderCode { get; set; }
        public IEnumerable<string> AvailableOptions { get; set; }
        public IEnumerable<string> AvailableCovers { get; set; }
    }
}