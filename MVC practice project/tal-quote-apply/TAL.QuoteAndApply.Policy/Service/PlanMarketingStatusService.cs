using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IPlanMarketingStatusService
    {
        IPlanMarketingStatus GetById(int id);
        IPlanMarketingStatus CreatePlanMarketingStatus(IPlanMarketingStatus planMarketingStatusDto);
        IPlanMarketingStatus GetPlanMarketingStatusByPlanId(int planId);
        void UpdatePlanMarketingStatus(IPlanMarketingStatus planMarketingStatusDto);
    }

    public class PlanMarketingStatusService : IPlanMarketingStatusService
    {
        private readonly IPlanMarketingStatusDtoRepository _planMarketingStatusDtoRepository;

        public PlanMarketingStatusService(IPlanMarketingStatusDtoRepository planMarketingStatusDtoRepository)
        {
            _planMarketingStatusDtoRepository = planMarketingStatusDtoRepository;
        }

        public IPlanMarketingStatus GetById(int id)
        {
            return _planMarketingStatusDtoRepository.GetPlanMarketingStatus(id);
        }

        public IPlanMarketingStatus CreatePlanMarketingStatus(IPlanMarketingStatus planMarketingStatusDto)
        {
            return _planMarketingStatusDtoRepository.InsertPlanMarketingStatus((PlanMarketingStatusDto)planMarketingStatusDto);
        }

        public IPlanMarketingStatus GetPlanMarketingStatusByPlanId(int planId)
        {
            return _planMarketingStatusDtoRepository.GetPlanMarketingStatusByPlanId(planId);
        }

        public void UpdatePlanMarketingStatus(IPlanMarketingStatus planMarketingStatusDto)
        {
            _planMarketingStatusDtoRepository.UpdatePlanMarketingStatus((PlanMarketingStatusDto)planMarketingStatusDto);
        }
    }
}
