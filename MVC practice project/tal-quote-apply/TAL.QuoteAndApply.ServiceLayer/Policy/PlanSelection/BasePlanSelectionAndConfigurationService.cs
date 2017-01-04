using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.PlanSelection
{
    public interface IPlanSelectionAndConfigurationService
    {
        void Run(IRisk risk);
    }

    public abstract class BasePlanSelectionAndConfigurationService : IPlanSelectionAndConfigurationService
    {
        private readonly IPlanService _planService;
        private readonly ICoverService _coverService;
        private readonly IOptionService _optionService;

        protected BasePlanSelectionAndConfigurationService(IPlanService planService, ICoverService coverService, IOptionService optionService)
        {
            _planService = planService;
            _coverService = coverService;
            _optionService = optionService;
        }

        public abstract void SetupSelections(IRisk risk, IEnumerable<IPlan> plans, IEnumerable<ICover> covers,
            IEnumerable<IOption> options);

        public void Run(IRisk risk)
        {
            var plans = _planService.GetPlansForRisk(risk.Id).ToArray();
            var covers = plans.SelectMany(p => _coverService.GetCoversForPlan(p.Id)).ToArray();
            var options = plans.SelectMany(p => _optionService.GetOptionsForPlan(p.Id)).ToArray();

            SetupSelections(risk, plans, covers, options);

            foreach (var option in options)
            {
                _optionService.UpdateOption(option);
            }

            foreach (var cover in covers)
            {
                _coverService.UpdateCover(cover);
            }

            foreach (var plan in plans)
            {
                _planService.UpdatePlan(plan);
            }
        }
    }
}
