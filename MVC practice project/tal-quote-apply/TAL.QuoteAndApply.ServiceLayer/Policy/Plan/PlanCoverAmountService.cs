using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Rules;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Rules;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanCoverAmountService
    {
        PlanCoverAmountToMaxResult ChangePlanCoverAmountToMinOrMaxIfApplicable(IRisk risk, IPlan plan,
            IEnumerable<IPlan> allPlans);
    }

    public class PlanCoverAmountService : IPlanCoverAmountService
    {
        private readonly IPlanService _planService;
        private readonly IMaxCoverAmountParamConverter _maxCoverAmountParamConverter;
        private readonly ICoverAmountService _coverAmountService;
        private readonly IRuleFactory _productRulesFactory;
        private readonly IPlanDefinitionProvider _planDefinitionProvider;
        private readonly IProductBrandProvider _productBrandProvider;

        public PlanCoverAmountService(IPlanService planService, IMaxCoverAmountParamConverter maxCoverAmountParamConverter, ICoverAmountService coverAmountService, IRuleFactory productRulesFactory, IPlanDefinitionProvider planDefinitionProvider, IProductBrandProvider productBrandProvider)
        {
            _maxCoverAmountParamConverter = maxCoverAmountParamConverter;
            _coverAmountService = coverAmountService;
            _productRulesFactory = productRulesFactory;
            _planDefinitionProvider = planDefinitionProvider;
            _productBrandProvider = productBrandProvider;
            _planService = planService;
        }

        public PlanCoverAmountToMaxResult ChangePlanCoverAmountToMinOrMaxIfApplicable(IRisk risk, IPlan plan, IEnumerable<IPlan> allPlans)
        {
            var parentPlan = _planService.GetParentPlanForPlan(plan, allPlans);
            var brandKey = _productBrandProvider.GetBrandKeyForRisk(risk);
            var planDefinition = _planDefinitionProvider.GetPlanByCode(plan.Code, brandKey);

            var maxCoverAmountParam = _maxCoverAmountParamConverter.CreateFrom(risk, plan, parentPlan, brandKey);

            bool coverAmountChanged = false;

            //scale to the max if we are over the max
            if (!_productRulesFactory.GetMaxCoverAmountRules(_coverAmountService, maxCoverAmountParam).AllAreSatisfied(plan.CoverAmount))
            {
                var maxCoverAmount = _coverAmountService.GetMaxCover(maxCoverAmountParam);
                plan.CoverAmount = maxCoverAmount;
                coverAmountChanged = true;
            }

            //scale to the min if we are under the min
            if (!_productRulesFactory.GetMinCoverAmountRules(planDefinition).AllAreSatisfied(plan.CoverAmount))
            {
                var minCoverAmount = _coverAmountService.GetMinCover(plan.Code, brandKey);
                plan.CoverAmount = minCoverAmount;
                coverAmountChanged = true;
            }

            return new PlanCoverAmountToMaxResult(plan, coverAmountChanged);
        }
    }
}