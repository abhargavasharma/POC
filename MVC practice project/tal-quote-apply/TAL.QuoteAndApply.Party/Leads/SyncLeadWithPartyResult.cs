
namespace TAL.QuoteAndApply.Party.Leads
{
    public class SyncLeadWithPartyResult
    {
        public SyncLeadResult SyncLeadResult { get; }
        public long? LeadId { get; }

        public SyncLeadWithPartyResult(SyncLeadResult syncLeadResult)
        {
            SyncLeadResult = syncLeadResult;
        }

        public SyncLeadWithPartyResult(SyncLeadResult syncLeadResult, long leadId)
        {
            SyncLeadResult = syncLeadResult;
            LeadId = leadId;
        }
    }
}
