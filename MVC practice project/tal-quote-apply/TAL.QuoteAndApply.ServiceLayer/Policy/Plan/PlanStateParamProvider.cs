using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanStateParamProvider
    {
        PlanStateParam CreateFrom(IRisk risk, IPlan plan, IEnumerable<IPlan> allPlans);
    }

    public class PlanStateParamProvider : IPlanStateParamProvider
    {
        private readonly IPlanService _planService;
        private readonly ICoverService _coverService;
        private readonly IOptionService _optionsService;
        private readonly IProductBrandProvider _productBrandProvider;

        public PlanStateParamProvider(IPlanService planService, ICoverService coverService, IOptionService optionsService, IProductBrandProvider productBrandProvider)
        {
            _planService = planService;
            _coverService = coverService;
            _optionsService = optionsService;
            _productBrandProvider = productBrandProvider;
        }

        public PlanStateParam CreateFrom(IRisk risk, IPlan plan, IEnumerable<IPlan> allPlans)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForRisk(risk);

            var covers = GetCoversForPlan(plan);
            var options = GetOptionsForPlan(plan);
            var parentPlans = _planService.GetParentPlansFromAllPlans(allPlans);
            var riders = _planService.GetRidersForParentPlan(plan, allPlans);

            var riderPlanStateParams = riders.Select(r => BuildRiderPlanStateParam(risk, r, plan, options));

            var parentPlanInfos = parentPlans.Select(p => new PlanIdentityInfo
            {
                PlanCode = p.Code,
                PlanId = p.Id,
                Selected = p.Selected
            });

            return PlanStateParam.BuildPlanStateParam(
                coverAmount: plan.CoverAmount,
                age: risk.DateOfBirth.Age(),
                income: risk.AnnualIncome,
                planId: plan.Id,
                selectedCoverCodes: covers.Where(x => x.Selected).Select(y => y.Code),
                riders: riderPlanStateParams,
                planOptions: options.Select(o => new OptionsParam(o.Code, o.Selected)),
                allPlans: parentPlanInfos,
                policyId: risk.PolicyId,
                riskId: risk.Id,
                planCode: plan.Code,
                linkedToCpi: plan.LinkedToCpi,
                selected: plan.Selected,
                premiumHoliday: plan.PremiumHoliday,
                premiumType: plan.PremiumType,
                waitingPeriod: plan.WaitingPeriod,
                benefitPeriod: plan.BenefitPeriod,
                occupationDefinition: plan.OccupationDefinition,
                brandKey: brandKey
            );
        }

        private PlanStateParam BuildRiderPlanStateParam(IRisk risk, IPlan rider, IPlan parentPlan, IEnumerable<IOption> parentPlanOptions)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForRisk(risk);

            var riderCovers = GetCoversForPlan(rider);

            return PlanStateParam.BuildRiderPlanStateParam(rider.Code,
               brandKey,
               rider.Selected,
               risk.PolicyId,
               risk.Id,
               parentPlan.LinkedToCpi,
               rider.CoverAmount,
               parentPlan.PremiumHoliday,
               parentPlan.PremiumType,
               rider.Id,
               risk.DateOfBirth.Age(),
               risk.AnnualIncome,
               parentPlanOptions.Select(o => new OptionsParam(o.Code, o.Selected)),
               riderCovers.Where(x => x.Selected).Select(y => y.Code),
               parentPlan.CoverAmount,
               rider.OccupationDefinition
               );
        }

        private IEnumerable<IOption> GetOptionsForPlan(IPlan plan)
        {
            return _optionsService.GetOptionsForPlan(plan.Id);
        }

        private IEnumerable<ICover> GetCoversForPlan(IPlan plan)
        {
            return _coverService.GetCoversForPlan(plan.Id);
        }
    }
}
