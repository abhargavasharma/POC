using System.Collections.Generic;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IPartyConsentResultConverter
    {
        PartyConsentResult CreateFrom(IRisk risk, IPartyConsent party);
    }

    public class PartyConsentResultConverter : IPartyConsentResultConverter
    {
        public PartyConsentResult CreateFrom(IRisk risk, IPartyConsent partyConsent)
        {
            var returnVal = new PartyConsentResult(partyConsent.PartyId);

            if (partyConsent != null)
            {
                returnVal.Consents = new List<string>();
                returnVal.Consents = UpdateConsentListIfConsentSelected(returnVal.Consents, ConsentType.DncEmail,
                    partyConsent.DncEmail);
                returnVal.Consents = UpdateConsentListIfConsentSelected(returnVal.Consents, ConsentType.DncHomeNumber,
                    partyConsent.DncHomeNumber);
                returnVal.Consents = UpdateConsentListIfConsentSelected(returnVal.Consents, ConsentType.DncMobile,
                    partyConsent.DncMobile);
                returnVal.Consents = UpdateConsentListIfConsentSelected(returnVal.Consents, ConsentType.DncPostalMail,
                    partyConsent.DncPostalMail);
                returnVal.ExpressConsent = partyConsent.ExpressConsent;
            }

            return returnVal;
        }

        public List<string> UpdateConsentListIfConsentSelected(List<string> consentList, ConsentType consentType, bool isSelected)
        {
            if (isSelected)
            {
                consentList.Add(consentType.ToString().ToCamelCase());
            }
            return consentList;
        }
    }

}
