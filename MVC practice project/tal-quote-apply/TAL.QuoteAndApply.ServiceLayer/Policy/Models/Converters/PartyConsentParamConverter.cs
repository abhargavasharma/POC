using System.Collections.Generic;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IPartyConsentParamConverter
    {
        PartyConsentParam CreateFrom(IPartyConsent party);
    }

    public class PartyConsentParamConverter : IPartyConsentParamConverter
    {
        public PartyConsentParam CreateFrom(IPartyConsent partyConsent)
        {
            var consentList = new List<string>();
            if (partyConsent != null)
            {
                consentList = UpdateConsentListIfConsentSelected(consentList, ConsentType.DncEmail,
                    partyConsent.DncEmail);
                consentList = UpdateConsentListIfConsentSelected(consentList, ConsentType.DncHomeNumber,
                    partyConsent.DncHomeNumber);
                consentList = UpdateConsentListIfConsentSelected(consentList, ConsentType.DncMobile,
                    partyConsent.DncMobile);
                consentList = UpdateConsentListIfConsentSelected(consentList, ConsentType.DncPostalMail,
                    partyConsent.DncPostalMail);
            }

            var returnVal = new PartyConsentParam(consentList, partyConsent?.ExpressConsent ?? false);

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
