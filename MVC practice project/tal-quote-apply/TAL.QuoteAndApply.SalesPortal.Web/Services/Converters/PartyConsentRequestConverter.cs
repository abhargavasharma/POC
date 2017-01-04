using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPartyConsentRequestConverter
    {
        PartyConsentRequest From(PartyConsentResult partyConsent);
        PartyConsentParam From(PartyConsentRequest partyConsent);
    }

    public class PartyConsentRequestConverter : IPartyConsentRequestConverter
    {
        public PartyConsentRequest From(PartyConsentResult partyConsent)
        {
            return new PartyConsentRequest()
            {
                Consents = partyConsent.Consents,
                ExpressConsent = partyConsent.ExpressConsent
            };
        }

        public PartyConsentParam From(PartyConsentRequest partyConsent)
        {
            return new PartyConsentParam(
                partyConsent.Consents, partyConsent.ExpressConsent);
        }
    }
}