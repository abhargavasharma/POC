using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanEligibilityProvider
    {
        IEnumerable<PlanEligibilityResult> GetPlanEligibilitiesFor(int riskId);
        PlanEligibilityResult GetPlanEligibilityFor(int riskId, int planId);
    }

    public class PlanEligibilityProvider : IPlanEligibilityProvider
    {
        private readonly IRiskService _riskService;
        private readonly IPlanService _planService;
        private readonly ICoverService _coverService;
        private readonly IPlanEligibilityService _planEligibilityService;
        private readonly ICoverEligibilityService _coverEligibilityService;

        public PlanEligibilityProvider(IRiskService riskService, IPlanService planService,
            IPlanEligibilityService planEligibilityService, ICoverEligibilityService coverEligibilityService,
            ICoverService coverService)
        {
            _riskService = riskService;
            _planService = planService;
            _planEligibilityService = planEligibilityService;
            _coverEligibilityService = coverEligibilityService;
            _coverService = coverService;
        }

        public IEnumerable<PlanEligibilityResult> GetPlanEligibilitiesFor(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            var allPlans = _planService.GetPlansForRisk(riskId);

            var parentPlans = _planService.GetParentPlansFromAllPlans(allPlans);
            
            var planEligibilityResults = new List<PlanEligibilityResult>();

            foreach (var plan in parentPlans)
            {
                var ridersForPlans = _planService.GetRidersForParentPlan(plan, allPlans);

                var riderEligibilities = new List<PlanEligibilityResult>();

                foreach (var rider in ridersForPlans)
                {
                    var riderCovers = _coverService.GetCoversForPlan(rider.Id);
                    var riderCoverEligibilityResults = _coverEligibilityService.GetCoverEligibilityResults(riderCovers);
                    var riderCoverEligibility = _planEligibilityService.IsRiskEligibleForPlan(risk, rider,
                        riderCoverEligibilityResults);

                    riderEligibilities.Add(new PlanEligibilityResult(rider.Code,
                        riderCoverEligibility.IsAvailable, null, riderCoverEligibilityResults, riderCoverEligibility.ReasonIfUnavailable));
                }

                var planCovers = _coverService.GetCoversForPlan(plan.Id);
                var planCoverEligibiltyResults = _coverEligibilityService.GetCoverEligibilityResults(planCovers);

                var riderEligibility = _planEligibilityService.IsRiskEligibleForPlan(risk, plan,
                    planCoverEligibiltyResults);

                var eligiblityResult = new PlanEligibilityResult(plan.Code,
                    riderEligibility.IsAvailable,
                    riderEligibilities, planCoverEligibiltyResults, riderEligibility.ReasonIfUnavailable);

                planEligibilityResults.Add(eligiblityResult);
            }

            return planEligibilityResults;
        }

        public PlanEligibilityResult GetPlanEligibilityFor(int riskId, int planId)
        {
            var risk = _riskService.GetRisk(riskId);
            var plan = _planService.GetById(planId);
            var planCovers = _coverService.GetCoversForPlan(plan.Id);
            var planCoverEligibiltyResults = _coverEligibilityService.GetCoverEligibilityResults(planCovers);
            var planCoverEligibility = _planEligibilityService.IsRiskEligibleForPlan(risk, plan,
                planCoverEligibiltyResults);

            return new PlanEligibilityResult(plan.Code,
                planCoverEligibility.IsAvailable, null,
                planCoverEligibiltyResults, planCoverEligibility.ReasonIfUnavailable);
        }
    }
}
