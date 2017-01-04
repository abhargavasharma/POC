using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IPolicyPremiumSummaryConverter
    {
        PolicyPremiumSummary CreateFrom(PolicyWithRisks policyWithRisks);
    }

    public class PolicyPremiumSummaryConverter : IPolicyPremiumSummaryConverter
    {
        private readonly IPlanService _planService;

        public PolicyPremiumSummaryConverter(IPlanService planService)
        {
            _planService = planService;
        }

        public PolicyPremiumSummary CreateFrom(PolicyWithRisks policyWithRisks)
        {
            var riskPremiumCalculationSummaries = new List<RiskPremiumSummary>();

            foreach (var riskWrapper in policyWithRisks.Risks)
            {
                var planCalculationSummaries = new List<PlanPremiumSummary>();
                var allPlans = riskWrapper.Plans.Select(p => p.Plan);

                foreach (var planWrapper in riskWrapper.Plans)
                {
                    var coverCalculationSummaries = new List<CoverPremiumSummary>();

                    foreach (var cover in planWrapper.Covers)
                    {
                        coverCalculationSummaries.Add(new CoverPremiumSummary(cover.Code, cover.Premium, policyWithRisks.Policy.PremiumFrequency));
                    }

                    var ridersForPlan = GetSelectedRidersForPlans(planWrapper.Plan, riskWrapper.Plans);

                    var totalPremiumIncludingRiders = ridersForPlan.Sum(p => p.Premium) + planWrapper.Plan.Premium;

                    var parentPlan = _planService.GetParentPlanForPlan(planWrapper.Plan, allPlans);
                    
                    planCalculationSummaries.Add(new PlanPremiumSummary(planWrapper.Plan.Code, planWrapper.Plan.Premium, totalPremiumIncludingRiders, planWrapper.Plan.MultiCoverDiscount, 
                        planWrapper.Plan.CoverAmount, _planService.IsPlanSelected(planWrapper.Plan, parentPlan), planWrapper.Plan.ParentPlanId != null, policyWithRisks.Policy.PremiumFrequency, coverCalculationSummaries));
                }

                riskPremiumCalculationSummaries.Add(new RiskPremiumSummary(riskWrapper.Risk.Id, riskWrapper.Risk.Premium, riskWrapper.Risk.MultiPlanDiscount, policyWithRisks.Policy.PremiumFrequency, planCalculationSummaries));
            }

            return new PolicyPremiumSummary(policyWithRisks.Policy.Premium, policyWithRisks.Policy.PremiumFrequency, riskPremiumCalculationSummaries);
        }

        private IReadOnlyList<IPlan> GetSelectedRidersForPlans(IPlan currentPlan, IEnumerable<PlanWithCovers> allPlans)
        {
            return allPlans.Where(p => p.Plan.ParentPlanId == currentPlan.Id && p.Plan.Selected).Select(p=> p.Plan).ToList();
        } 
    }
}