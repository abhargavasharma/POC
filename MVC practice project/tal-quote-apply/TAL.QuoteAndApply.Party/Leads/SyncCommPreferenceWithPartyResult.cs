using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Party.Leads
{
    public class SyncCommPreferenceWithPartyResult
    {
        public SyncCommPreferenceResult SyncCommPreferenceResult { get; }
        public long? CommPreferenceId { get; }

        public SyncCommPreferenceWithPartyResult(SyncCommPreferenceResult syncCommPreferenceResult)
        {
            SyncCommPreferenceResult = syncCommPreferenceResult;
        }

        public SyncCommPreferenceWithPartyResult(SyncCommPreferenceResult syncCommPreferenceResult, long adobeId)
        {
            SyncCommPreferenceResult = syncCommPreferenceResult;
            CommPreferenceId = adobeId;
        }
    }
}
