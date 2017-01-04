using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PolicyInteractionsResult
    {
        public IReadOnlyList<PolicyInteractions> Interactions { get; }

        public PolicyInteractionsResult(IReadOnlyList<PolicyInteractions> interactions)
        {
            Interactions = interactions;
        }
    }
}
