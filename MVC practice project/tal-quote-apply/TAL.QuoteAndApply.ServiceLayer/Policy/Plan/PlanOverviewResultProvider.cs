using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Plan.Models.Mappers;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanOverviewResultProvider
    {
        IEnumerable<PlanOverviewResult> GetFor(int riskId);
    }

    public class PlanOverviewResultProvider : IPlanOverviewResultProvider
    {
        private readonly IPlanService _planService;
        private readonly IPlanDtoConverter _planDtoConverter;

        public PlanOverviewResultProvider(IPlanService planService, IPlanDtoConverter planDtoConverter)
        {
            _planService = planService;
            _planDtoConverter = planDtoConverter;
        }

        public IEnumerable<PlanOverviewResult> GetFor(int riskId)
        {
            var plans = _planService.GetPlansForRisk(riskId);

            var plansDto = _planDtoConverter.CreateTo(plans);

            return plansDto;
        }
    }
}
