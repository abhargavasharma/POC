using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PlanCalculationRequest : IPlanFactors
    {
        public string PlanCode { get;  }
        public int? WaitingPeriod { get; }
        public int? BenefitPeriod { get; }
        public OccupationDefinition OccupationDefinition { get; }
        public decimal? OccupationLoading { get; }

        public bool? BuyBack { get; }
        public PremiumType PremiumType { get; }
        public bool IncludeInMultiPlanDiscount { get; private set; }
        public bool? PremiumReliefOptionSelected { get; }
        public bool? IncreasingClaimsSelected { get; }
        public bool? DayOneAccidentSelected { get; }
        public decimal CoverAmount { get;  }
        public IReadOnlyList<CoverCalculationRequest> Covers { get; }
        public bool Active { get; }

        public PlanCalculationRequest(string planCode, bool active, bool includeInMultiPlanDiscount, 
            decimal coverAmount, PremiumType premiumType, bool? buyBack, int? waitingPeriod, 
            int? benefitPeriod, OccupationDefinition occupationDefinition, decimal? occupationLoading, bool? premiumReliefOptionSelected,
            bool? increasingClaimsSelected, bool? dayOneAccidentSelected,
            IReadOnlyList<CoverCalculationRequest> covers)
        {
            PlanCode = planCode;
            WaitingPeriod = waitingPeriod;
            BenefitPeriod = benefitPeriod;
            OccupationDefinition = occupationDefinition;
            OccupationLoading = occupationLoading;
            PremiumReliefOptionSelected = premiumReliefOptionSelected;
            IncreasingClaimsSelected = increasingClaimsSelected;
            DayOneAccidentSelected = dayOneAccidentSelected;
            CoverAmount = coverAmount;
            Covers = covers;
            IncludeInMultiPlanDiscount = includeInMultiPlanDiscount;
            PremiumType = premiumType;
            BuyBack = buyBack;
            Active = active;
        }

        public PlanCalculationRequest(PlanCalculationRequest pcr, IReadOnlyList<CoverCalculationRequest> coverCalculationRequests) 
            : this(pcr.PlanCode, pcr.Active, pcr.IncludeInMultiPlanDiscount, pcr.CoverAmount, pcr.PremiumType, pcr.BuyBack,
            pcr.WaitingPeriod, pcr.BenefitPeriod, pcr.OccupationDefinition, pcr.OccupationLoading, pcr.PremiumReliefOptionSelected, 
            pcr.IncreasingClaimsSelected, pcr.DayOneAccidentSelected, coverCalculationRequests)
        { }

        public PlanCalculationRequest WithExcludeFromMultiPlanDiscount()
        {
            IncludeInMultiPlanDiscount = false;
            return this;
        }
    }
}