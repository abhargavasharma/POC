using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.PlanSelection
{
    public class SetNothingPlanSelectionAndConfiguration : BasePlanSelectionAndConfigurationService
    {
        public SetNothingPlanSelectionAndConfiguration(IPlanService planService, ICoverService coverService, IOptionService optionService) : base(planService, coverService, optionService)
        {
        }

        public override void SetupSelections(IRisk risk, IEnumerable<IPlan> plans, IEnumerable<ICover> covers,
            IEnumerable<IOption> options)
        {

        }
    }
}