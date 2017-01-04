using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Party.Data;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.Party.Services
{
    public interface IPartyConsentService
    {
        IPartyConsent CreatePartyConsent(IPartyConsent partyConsent, IParty party);
        void UpdatePartyConsent(IPartyConsent partyConsent, IParty party);
        IPartyConsent GetPartyConsent(int partyConsentId);
        IPartyConsent GetPartyConsentByPartyId(int partyId);
    }

    public class PartyConsentService : IPartyConsentService
    {
        private readonly IPartyConsentDtoRepository _partyConsentDtoRepository;
        private readonly ISyncCommPreferenceService _syncCommPreferenceService;

        public PartyConsentService(IPartyConsentDtoRepository partyConsentDtoRepository, ISyncCommPreferenceService syncCommPreferenceService)
        {
            _partyConsentDtoRepository = partyConsentDtoRepository;
            _syncCommPreferenceService = syncCommPreferenceService;
        }

        public void UpdatePartyConsent(IPartyConsent partyConsent, IParty party)
        {
            var partyConsentDto = (PartyConsentDto)partyConsent;

            _partyConsentDtoRepository.UpdatePartyConsent(partyConsentDto);

            _syncCommPreferenceService.SyncCommPreferenceWithParty(party, partyConsent);
        }

        public IPartyConsent GetPartyConsent(int partyConsentId)
        {
            return _partyConsentDtoRepository.GetPartyConsent(partyConsentId);
        }

        public IPartyConsent GetPartyConsentByPartyId(int partyId)
        {
            return _partyConsentDtoRepository.GetPartyConsentByPartyId(partyId);
        }

        public IPartyConsent CreatePartyConsent(IPartyConsent partyConsent, IParty party)
        {
            var returnPartyConsent = _partyConsentDtoRepository.InsertPartyConsent((PartyConsentDto)partyConsent);

            _syncCommPreferenceService.SyncCommPreferenceWithParty(party, partyConsent);

            return returnPartyConsent;
        }
    }
}
