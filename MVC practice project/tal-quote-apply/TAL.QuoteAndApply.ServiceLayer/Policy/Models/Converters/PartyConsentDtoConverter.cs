using System;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IPartyConsentDtoConverter
    {
        IPartyConsent CreateFrom(int partyId);
    }
    public interface IPartyConsentDtoUpdater
    {
        IPartyConsent UpdateFrom(IPartyConsent partyConsent, PartyConsentParam partyConsentParam);
        IPartyConsent UpdateFrom(IPartyConsent partyConsentDto, GetPreferredCommunicationResult getCommunicationPreferences);
    }

    public class PartyConsentDtoConverter : IPartyConsentDtoConverter, IPartyConsentDtoUpdater
    {
        public IPartyConsent CreateFrom(int partyId)
        {
            return new PartyConsentDto(partyId);
        }

        public IPartyConsent UpdateFrom(IPartyConsent partyConsent, PartyConsentParam partyConsentParam)
        {
            partyConsent.ExpressConsent = partyConsentParam.ExpressConsent;
            partyConsent.DncEmail = partyConsentParam.Consents.Contains(ConsentType.DncEmail.ToString().ToCamelCase());
            partyConsent.DncHomeNumber = partyConsentParam.Consents.Contains(ConsentType.DncHomeNumber.ToString().ToCamelCase());
            partyConsent.DncMobile = partyConsentParam.Consents.Contains(ConsentType.DncMobile.ToString().ToCamelCase());
            partyConsent.DncPostalMail = partyConsentParam.Consents.Contains(ConsentType.DncPostalMail.ToString().ToCamelCase());
            return partyConsent;
        }

        public IPartyConsent UpdateFrom(IPartyConsent partyConsentDto, GetPreferredCommunicationResult getCommunicationPreferences)
        {
            partyConsentDto.ExpressConsent = getCommunicationPreferences.ExpressConsent;
            partyConsentDto.DncEmail = getCommunicationPreferences.DncEmail;
            partyConsentDto.DncHomeNumber = getCommunicationPreferences.DncHomeNumber;
            partyConsentDto.DncMobile = getCommunicationPreferences.DncMobile;
            partyConsentDto.DncPostalMail = getCommunicationPreferences.DncPostalMail;
            partyConsentDto.ExpressConsentUpdatedTs = getCommunicationPreferences.ExpressConsentUpdatedTs?.Year < 1980 ? null : getCommunicationPreferences.ExpressConsentUpdatedTs;
            return partyConsentDto;

        }
    }
}
