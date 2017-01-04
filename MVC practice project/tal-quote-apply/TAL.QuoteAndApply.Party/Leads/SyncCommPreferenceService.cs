using TAL.QuoteAndApply.Party.Leads.LeadsService;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.Party.Leads
{
    public interface ISyncCommPreferenceService
    {
        SyncCommPreferenceWithPartyResult SyncCommPreferenceWithParty(IParty party, IPartyConsent partyConsent);

        SyncCommPreferenceWithPartyResult SyncCommPreferenceWithParty(IParty party, IPartyConsent partyConsent,
            long? leadId);
    }
    public class SyncCommPreferenceService : ISyncCommPreferenceService
    {
        private readonly IHttpLeadsService _httpLeadsService;
        private readonly IPartyCommunicationMessageConverter _partyCommunicationMessageConverter;

        public SyncCommPreferenceService(IHttpLeadsService httpLeadsService, IPartyCommunicationMessageConverter partyCommunicationMessageConverter)
        {
            _httpLeadsService = httpLeadsService;
            _partyCommunicationMessageConverter = partyCommunicationMessageConverter;
        }

        public SyncCommPreferenceWithPartyResult SyncCommPreferenceWithParty(IParty party, IPartyConsent partyConsent)
        {
            return SyncCommPreferenceWithParty(party, partyConsent, party.LeadId);
        }

        public SyncCommPreferenceWithPartyResult SyncCommPreferenceWithParty(IParty party, IPartyConsent partyConsent, long? leadId)
        {
            if (leadId.HasValue)
            {
                var convertedMsg = _partyCommunicationMessageConverter.From(party, partyConsent);
                var updateCommunicationPreferenceResult = _httpLeadsService.UpdateCommunicationPreferences(convertedMsg);
                if (updateCommunicationPreferenceResult != null)
                {
                    return new SyncCommPreferenceWithPartyResult(SyncCommPreferenceResult.CommPreferenceLeadUpdated,
                        party.LeadId.Value);
                }
            }

            return new SyncCommPreferenceWithPartyResult(SyncCommPreferenceResult.NoActionPerformed);
        }
    }
}
