using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Leads.LeadsService;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Party.Services;

namespace TAL.QuoteAndApply.Party.Leads
{
    public interface ISyncLeadService
    {
        SyncLeadWithPartyResult SyncLeadWithParty(IParty party, PolicySource policySource);
    }

    public class SyncLeadService : ISyncLeadService
    {
        private readonly IPartyRulesService _partyRulesService;
        private readonly IHttpLeadsService _httpLeadsService;
        private readonly IPartyToLeadsMessageConverter _partyToLeadsMessageConverter;
        private readonly IPartyConsentService _partyConsentService;
        private readonly ISyncCommPreferenceService _syncCommPreferenceService;

        public SyncLeadService(IPartyRulesService partyRulesService, IHttpLeadsService httpLeadsService, 
            IPartyToLeadsMessageConverter partyToLeadsMessageConverter, IPartyConsentService partyConsentService,
            ISyncCommPreferenceService syncCommPreferenceService)
        {
            _partyRulesService = partyRulesService;
            _httpLeadsService = httpLeadsService;
            _partyToLeadsMessageConverter = partyToLeadsMessageConverter;
            _partyConsentService = partyConsentService;
            _syncCommPreferenceService = syncCommPreferenceService;
        }

        public SyncLeadWithPartyResult SyncLeadWithParty(IParty party, PolicySource policySource)
        {
            if (party.LeadId.HasValue)
            {
                var message = _partyToLeadsMessageConverter.From(party, policySource);
                var response = _httpLeadsService.UpdateLead(message);
                if (response != null)
                {
                    return new SyncLeadWithPartyResult(SyncLeadResult.LeadUpdated, response.AdobeId);
                }
            }
            else if (_partyRulesService.ValidatePartyForCreateLead(party).All(r => r.IsSatisfied))
            {
                var message = _partyToLeadsMessageConverter.From(party, policySource);
                var response = _httpLeadsService.CreateLead(message);
                if (response != null)
                {
                    party.LeadId = response.AdobeId;

                    var partyConsent = _partyConsentService.GetPartyConsentByPartyId(party.Id);

                    if (partyConsent != null)
                    {
                        _syncCommPreferenceService.SyncCommPreferenceWithParty(party, partyConsent);
                    }
                    return new SyncLeadWithPartyResult(SyncLeadResult.LeadCreated, response.AdobeId);
                }
            }

            return new SyncLeadWithPartyResult(SyncLeadResult.NoActionPerformed);
        }
    }
}
