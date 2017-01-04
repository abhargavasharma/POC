using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Policy.Service.MarketingStatus;
using Status = TAL.QuoteAndApply.DataModel.Policy.MarketingStatus;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus
{
    public interface IPlanMarketingStatusUpdater
    {
        void CreateNewPlanMarketingStatus(IPlan plan, Status marketingStatus);
        Status UpdatePlanMarketingStatus(IPlan plan, bool planEligibile, Status[] coversMarketingStatusList);
    }

    public class PlanMarketingStatusUpdater : IPlanMarketingStatusUpdater
    {
        private readonly IPlanMarketingStatusService _planMarketingStatusService;
        private readonly IPlanMarketingStatusProvider _planMarketingStatusProvider;

        public PlanMarketingStatusUpdater(
            IPlanMarketingStatusService planMarketingStatusService,
            IPlanMarketingStatusProvider planMarketingStatusProvider)
        {
            _planMarketingStatusService = planMarketingStatusService;
            _planMarketingStatusProvider = planMarketingStatusProvider;
        }

        public void CreateNewPlanMarketingStatus(IPlan plan, Status marketingStatus)
        {
            var planMarketingStatusDto = new PlanMarketingStatusDto()
            {
                PlanId = plan.Id,
                MarketingStatusId = marketingStatus
            };
            _planMarketingStatusService.CreatePlanMarketingStatus(planMarketingStatusDto);
        }

        public Status UpdatePlanMarketingStatus(IPlan plan, bool planEligibile, Status[] coversMarketingStatusList)
        {
            var marketingStatus = _planMarketingStatusProvider.GetPlanMarketingStatus(plan.Selected, planEligibile, coversMarketingStatusList);
            var planMarketingStatus = _planMarketingStatusService.GetPlanMarketingStatusByPlanId(plan.Id);
            if (planMarketingStatus == null)
            {
                CreateNewPlanMarketingStatus(plan, Status.Unknown);
                planMarketingStatus = _planMarketingStatusService.GetPlanMarketingStatusByPlanId(plan.Id);
            }

            planMarketingStatus.MarketingStatusId = marketingStatus;
            _planMarketingStatusService.UpdatePlanMarketingStatus(planMarketingStatus);
            return marketingStatus;
        }
    }
}
