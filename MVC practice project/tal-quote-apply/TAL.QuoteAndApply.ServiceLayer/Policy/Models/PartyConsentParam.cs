using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PartyConsentParam
    {
        public List<string> Consents { get; private set; } 
        public bool ExpressConsent { get; private set; }

        public PartyConsentParam(List<string> consents, bool expressConsent)
        {
            Consents = consents;
            ExpressConsent = expressConsent;
        }
    }
}
