
using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PartyConsentResult
    {
        public int PartyId { get; set; }
        public List<string> Consents { get; set; }
        public bool ExpressConsent { get; set; }

        public PartyConsentResult(int partyId)
        {
            PartyId = partyId;
        }
    }
}
