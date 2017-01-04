using System.Collections.Generic;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class PolicyFremiumFrequencyViewModel
    {
        public string QuoteReferenceNumber { get; set; }
        public string PremiumFrequency { get; set; }
        public IEnumerable<string> AvailableFrequencies { get; set; }
    }
}