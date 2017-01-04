using System.Collections.Generic;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class PartyConsentRequest
    {
        public List<string> Consents { get; set; } 
        public bool ExpressConsent { get; set; }
    }
}