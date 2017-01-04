using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PlanEligibilityResult
    {
        public string PlanCode { get; }
        public bool EligibleForPlan { get; }
        public IEnumerable<CoverEligibilityResult> CoverEligibilityResults { get; }
        public IEnumerable<PlanEligibilityResult> RiderEligibilityResults { get; }
        public IEnumerable<string> IneligibleReasons { get; }

        public PlanEligibilityResult(string planCode, bool eligibleForPlan,
            IEnumerable<PlanEligibilityResult> riderEligibilityResults,
            IEnumerable<CoverEligibilityResult> coverEligibilityResults, IEnumerable<string> ineligibleReasons)
        {
            PlanCode = planCode;
            EligibleForPlan = eligibleForPlan;
            RiderEligibilityResults = riderEligibilityResults;
            CoverEligibilityResults = coverEligibilityResults;
            IneligibleReasons = ineligibleReasons;
        }
    }
}
