using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Data;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.Party.Services
{
    public interface IPartyService
    {
        IParty CreatePartyWithoutLead(IParty party);
        IParty CreateParty(IParty party, PolicySource policySource);
        void UpdateParty(IParty party, PolicySource policySource);
        IParty GetParty(int partyId);
        bool IsPartyValidForInforce(IParty party);
        IList<IParty> GetPartiesByLeadId(long leadId);
        void DeleteParty(int partyId);
    }

    public class PartyService : IPartyService
    {
        private readonly IPartyRulesService _partyRulesService;
        private readonly IPartyDtoRepository _partyDtoRepository;
        private readonly ISyncLeadService _syncLeadService;
        private readonly IPartyConsentDtoRepository _partyConsentDtoRepository;

        public PartyService(IPartyRulesService partyRulesService, IPartyDtoRepository partyDtoRepository, ISyncLeadService syncLeadService, IPartyConsentDtoRepository partyConsentDtoRepository)
        {
            _partyRulesService = partyRulesService;
            _partyDtoRepository = partyDtoRepository;
            _syncLeadService = syncLeadService;
            _partyConsentDtoRepository = partyConsentDtoRepository;
        }

        public void UpdateParty(IParty party, PolicySource policySource)
        {
            var partyDto = (PartyDto)party;

            _partyDtoRepository.UpdateParty(partyDto);
            
            var syncLeadWithPartyResult = _syncLeadService.SyncLeadWithParty(partyDto, policySource);

            TagPartyWithLeadId(party, syncLeadWithPartyResult);
        }

        public IParty GetParty(int partyId)
        {
            return _partyDtoRepository.GetParty(partyId);
        }

        public bool IsPartyValidForInforce(IParty party)
        {
            return _partyRulesService.ValidatePartyForInforce(party).All(x=> x.IsSatisfied);
        }

        public IList<IParty> GetPartiesByLeadId(long leadId)
        {
            //todo: this seems shit, not sure if there is a better way
            return _partyDtoRepository.GetPartiesByLeadId(leadId).Select(party=> (IParty)party).ToList();
        }

        public IParty CreatePartyWithoutLead(IParty party)
        {
            return _partyDtoRepository.InsertParty((PartyDto)party);
        }

        public IParty CreateParty(IParty party, PolicySource policySource)
        {
            var result = CreatePartyWithoutLead(party);
            var syncLeadWithPartyResult = _syncLeadService.SyncLeadWithParty(result, policySource);
            return TagPartyWithLeadId(result, syncLeadWithPartyResult);
        }

        public void DeleteParty(int partyId)
        {
            _partyConsentDtoRepository.DeletePartyConsent(partyId);
            _partyDtoRepository.DeleteParty(partyId);
        }

        private IParty TagPartyWithLeadId(IParty party, SyncLeadWithPartyResult syncLeadWithPartyResult)
        {
            var partyDto = (PartyDto) party;

            if (syncLeadWithPartyResult.SyncLeadResult == SyncLeadResult.LeadCreated)
            {
                party.LeadId = syncLeadWithPartyResult.LeadId.Value;
                _partyDtoRepository.UpdateParty(partyDto);
            }

            return partyDto;
        }
    }
}
