using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PolicyPremiumFrequencyParam
    {
        public string QuoteReferenceNumber { get; }
        public PremiumFrequency PremiumFrequency { get; }
        public IEnumerable<string> AvailableFrequencies { get; }

        public PolicyPremiumFrequencyParam(string quoteReferenceNumber, PremiumFrequency premiumFrequency, IEnumerable<string> availableFrequencies)
        {
            QuoteReferenceNumber = quoteReferenceNumber;
            PremiumFrequency = premiumFrequency;
            AvailableFrequencies = availableFrequencies;
        }
    }
}
