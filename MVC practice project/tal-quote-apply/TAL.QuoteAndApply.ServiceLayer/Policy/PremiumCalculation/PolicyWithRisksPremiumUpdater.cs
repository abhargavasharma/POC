using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation
{
    public interface IPolicyWithRisksPremiumUpdater
    {
        void UpdatePremiumsOnPolicyWithRisks(PolicyWithRisks policyWithRisks, PremiumCalculationResult calculationResult);
    }

    public class PolicyWithRisksPremiumUpdater : IPolicyWithRisksPremiumUpdater
    {
        public void UpdatePremiumsOnPolicyWithRisks(PolicyWithRisks policyWithRisks, PremiumCalculationResult calculationResult)
        {
            foreach (var riskWrapper in policyWithRisks.Risks)
            {
                UpdateRisk(riskWrapper, calculationResult);
            }

            policyWithRisks.Policy.Premium = policyWithRisks.Risks.Sum(r => r.Risk.Premium).RoundToTwoDecimalPlaces();
        }

        private void UpdateRisk(RiskWithPlans riskWithPlans, PremiumCalculationResult calculationResult)
        {
            var riskPremiumCalcResult =
                calculationResult.RiskPremiumCalculationResults.FirstOrDefault(r => r.RiskId == riskWithPlans.Risk.Id);

            if (riskPremiumCalcResult == null)
            {
                return;
            }

            foreach (var planWrapper in riskWithPlans.Plans)
            {
                UpdatePlan(planWrapper, riskPremiumCalcResult);
            }

            riskWithPlans.Risk.Premium = riskPremiumCalcResult.RiskPremium;
            riskWithPlans.Risk.MultiPlanDiscount = riskPremiumCalcResult.MultiPlanDiscount;
        }

        private void UpdatePlan(PlanWithCovers planWithCovers,
            RiskPremiumCalculationResult riskPremiumCalculationResult)
        {
            var planPremiumCalcResult =
                riskPremiumCalculationResult.PlanPremiumCalculationResults.FirstOrDefault(
                    p => p.PlanCode == planWithCovers.Plan.Code);

            if (planPremiumCalcResult == null)
            {
                return;
            }

            foreach (var cover in planWithCovers.Covers)
            {
                UpdateCover(cover, planPremiumCalcResult);
            }

            //Plan premium set from result instead of sum of covers because it could contain discounts
            planWithCovers.Plan.Premium = planPremiumCalcResult.TotalPremium;
            planWithCovers.Plan.MultiCoverDiscount = planPremiumCalcResult.MultiCoverDiscount;
            planWithCovers.Plan.MultiPlanDiscount = planPremiumCalcResult.MultiPlanDiscount;
            planWithCovers.Plan.MultiPlanDiscountFactor = planPremiumCalcResult.MultiPlanDiscountFactor;
        }

        private void UpdateCover(ICover cover, PlanPremiumCalculationResult planPremiumCalculationResult)
        {
            var coverPremiumCalcResult =
                planPremiumCalculationResult.CoverPremiumCalculationResults.FirstOrDefault(
                    c => c.CoverCode == cover.Code);

            if (coverPremiumCalcResult == null)
            {
                return;
            }

            cover.Premium = coverPremiumCalcResult.TotalPremium;
        }
    }
}