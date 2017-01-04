using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
   
    public class PolicyPremiumSummary
    {
        public decimal PolicyPremium { get; }
        public PremiumFrequency PremiumFrequency { get; }
        public IEnumerable<RiskPremiumSummary> RiskPremiums { get; }

        public PolicyPremiumSummary(decimal policyPremium, PremiumFrequency premiumFrequency, IEnumerable<RiskPremiumSummary> riskPremiums)
        {
            PolicyPremium = policyPremium;
            PremiumFrequency = premiumFrequency;
            RiskPremiums = riskPremiums;
        }
    }

    public class RiskPremiumSummary
    {
        public int RiskId { get; }
        public decimal RiskPremium { get; }
        public decimal MultiPlanDiscount { get; }
        public PremiumFrequency PremiumFrequency { get; }
        public IEnumerable<PlanPremiumSummary> PlanPremiums { get; }

        public RiskPremiumSummary(int riskId, decimal riskPremium, decimal multiPlanDiscount, PremiumFrequency premiumFrequency, IEnumerable<PlanPremiumSummary> planPremiums)
        {
            RiskId = riskId;
            RiskPremium = riskPremium;
            MultiPlanDiscount = multiPlanDiscount;
            PremiumFrequency = premiumFrequency;
            PlanPremiums = planPremiums;
        }
    }

    public class PlanPremiumSummary
    {
        public string PlanCode { get; }
        public decimal PlanPremium { get; }
        public decimal PlanIncludingRidersPremium { get; }
        public decimal MultiCoverDiscount { get; }
        public decimal CoverAmount { get; }
        public bool PlanSelected { get; }
        public bool IsRider { get; }
        public PremiumFrequency PremiumFrequency { get; }
        public IEnumerable<CoverPremiumSummary> CoverPremiums { get; }

        public PlanPremiumSummary(string planCode, decimal planPremium, decimal planIncludingRidersPremium, decimal multiCoverDiscount, decimal coverAmount, bool planSelected,  bool isRider, PremiumFrequency premiumFrequency, IEnumerable<CoverPremiumSummary> coverPremiums)
        {
            PlanCode = planCode;
            PlanPremium = planPremium;
            PlanIncludingRidersPremium = planIncludingRidersPremium;
            MultiCoverDiscount = multiCoverDiscount;
            CoverAmount = coverAmount;
            PlanSelected = planSelected;
            IsRider = isRider;
            PremiumFrequency = premiumFrequency;
            CoverPremiums = coverPremiums;
        }
    }

    public class CoverPremiumSummary
    {
        public string CoverCode { get; }
        public decimal CoverPremium { get;  }
        public PremiumFrequency PremiumFrequency { get; }

        public CoverPremiumSummary(string coverCode, decimal coverPremium, PremiumFrequency premiumFrequency)
        {
            CoverCode = coverCode;
            CoverPremium = coverPremium;
            PremiumFrequency = premiumFrequency;
        }
    }
}
