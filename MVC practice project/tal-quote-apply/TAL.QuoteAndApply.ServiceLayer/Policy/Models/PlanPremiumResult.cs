using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PlanPremiumResult
    {
        public int PlanId { get; }
        public string PlanCode { get; }
        public decimal Premium { get; }
        public decimal PremiumIncludingRiders { get; }
        public PremiumFrequency PremiumFrequency { get; }
        public IReadOnlyList<CoverPremiumResult> CoverPremiumResults { get; }

        public bool ReadyForInForce
        {
            get { return Status == PlanStatus.Completed; }
        }

        public PlanStatus Status { get; set; }

        public PlanPremiumResult(int planId, string planCode, decimal premium, decimal premiumIncludingRiders,
            PremiumFrequency premiumFrequency, IReadOnlyList<CoverPremiumResult> coverPremiumResults, PlanStatus status)
        {
            PlanCode = planCode;
            PlanId = planId;
            PremiumFrequency = premiumFrequency;
            CoverPremiumResults = coverPremiumResults;
            Premium = premium;
            PremiumIncludingRiders = premiumIncludingRiders;
            Status = status;
        }
    }

    public class CoverPremiumResult
    {
        public string CoverCode { get; }
        public decimal Premium { get; }

        public CoverPremiumResult(string coverCode, decimal premium)
        {
            CoverCode = coverCode;
            Premium = premium;
        }
    }

}
