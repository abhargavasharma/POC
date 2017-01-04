using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.Tests.Shared.Mocks
{
    public class MockSyncLeadService : ISyncLeadService
    {
        public SyncLeadWithPartyResult SyncLeadWithParty(IParty party, PolicySource policySource)
        {
            return new SyncLeadWithPartyResult(SyncLeadResult.NoActionPerformed);
        }
    }
}