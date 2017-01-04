using System.Linq;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus
{
    public interface IUpdateMarketingStatusService
    {
        void UpdateMarketingStatusForRisk(int risk);
    }

    public class UpdateMarketingStatusService : IUpdateMarketingStatusService
    {
        private readonly IPlanService _planService;
        private readonly IPlanEligibilityService _planEligibilityService;
        private readonly ICoverService _coverService;
        private readonly ICoverEligibilityService _coverEligibilityService;
        private readonly ICoverMarketingStatusUpdater _coverMarketingStatusUpdater;
        private readonly IPlanMarketingStatusUpdater _planMarketingStatusUpdater;
        private readonly IRiskMarketingStatusUpdater _riskMarketingStatusUpdater;
        private readonly IRiskService _riskService;

        public UpdateMarketingStatusService(IPlanService planService, 
            IPlanEligibilityService planEligibilityService, 
            ICoverService coverService, 
            ICoverEligibilityService coverEligibilityService, 
            IRiskService riskService, 
            ICoverMarketingStatusUpdater coverMarketingStatusUpdater, 
            IPlanMarketingStatusUpdater planMarketingStatusUpdater, 
            IRiskMarketingStatusUpdater riskMarketingStatusUpdater)
        {
            _planService = planService;
            _planEligibilityService = planEligibilityService;
            _coverService = coverService;
            _coverEligibilityService = coverEligibilityService;
            _riskService = riskService;
            _coverMarketingStatusUpdater = coverMarketingStatusUpdater;
            _planMarketingStatusUpdater = planMarketingStatusUpdater;
            _riskMarketingStatusUpdater = riskMarketingStatusUpdater;
        }

        public void UpdateMarketingStatusForRisk(int riskId)
        {

            var plans = _planService.GetPlansForRisk(riskId);
            var risk = _riskService.GetRisk(riskId);

            var plansMarketingStatusList = (from plan in plans let planCovers = _coverService.GetCoversForPlan(plan.Id)
                                            let planCoverEligibiltyResults = _coverEligibilityService.GetCoverEligibilityResults(planCovers)
                                            let planEligible = _planEligibilityService.IsRiskEligibleForPlan(risk, plan, planCoverEligibiltyResults).IsAvailable
                                            let coversMarketingStatusList = (from cover in planCovers
                                                                             let coverEligibility = planCoverEligibiltyResults.Single(e => e.CoverCode == cover.Code)
                                                                             select _coverMarketingStatusUpdater.UpdateCoverMarketingStatus(cover, coverEligibility.EligibleForCover)).ToArray()
                                            select _planMarketingStatusUpdater.UpdatePlanMarketingStatus(plan, planEligible, coversMarketingStatusList)).ToArray();
            _riskMarketingStatusUpdater.UpdateRiskMarketingStatus(risk, plansMarketingStatusList);
        }
    }
}
