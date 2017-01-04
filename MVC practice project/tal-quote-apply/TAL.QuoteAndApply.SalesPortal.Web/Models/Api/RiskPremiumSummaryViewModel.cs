using System.Collections.Generic;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class RiskPremiumSummaryViewModel
    {
        public decimal Premium { get; }
        public string PremiumFrequency { get; }
        public decimal MultiPlanDiscount { get; }
        public IReadOnlyList<PlanPremiumSummaryViewModel> PlanPremiums { get; }

        public RiskPremiumSummaryViewModel(decimal premium, string premiumFrequency, decimal multiPlanDiscount, IReadOnlyList<PlanPremiumSummaryViewModel> planPremiums)
        {
            Premium = premium;
            MultiPlanDiscount = multiPlanDiscount;
            PlanPremiums = planPremiums;
            PremiumFrequency = premiumFrequency;
        }
    }

    public class PlanPremiumSummaryViewModel
    {
        public string PlanName { get; }
        public decimal Premium { get; }
        public decimal CoverAmount { get; }
        public bool IsRider { get; }

        public PlanPremiumSummaryViewModel(string planName, decimal premium, decimal coverAmount, bool isRider)
        {
            PlanName = planName;
            Premium = premium;
            CoverAmount = coverAmount;
            IsRider = isRider;
        }
    }
}