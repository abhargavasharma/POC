using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;

namespace TAL.QuoteAndApply.ServiceLayer.Models
{
    public class AvailableRiderOptionsAndConfigResult
    {
        public string RiderCode { get; set; }
        public IEnumerable<string> AvailableOptions { get; set; }
        public IEnumerable<string> AvailableCovers { get; set; }

        public IEnumerable<AvailableFeature> UnavailableFeatures { get; set; }
    }
}