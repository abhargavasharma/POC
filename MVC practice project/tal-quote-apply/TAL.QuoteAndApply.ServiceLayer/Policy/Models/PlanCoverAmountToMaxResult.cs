using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PlanCoverAmountToMaxResult
    {
        public IPlan Plan { get; }
        public bool CoverAmountChanged { get; }

        public PlanCoverAmountToMaxResult(IPlan plan, bool coverAmountChanged)
        {
            Plan = plan;
            CoverAmountChanged = coverAmountChanged;
        }
    }
}