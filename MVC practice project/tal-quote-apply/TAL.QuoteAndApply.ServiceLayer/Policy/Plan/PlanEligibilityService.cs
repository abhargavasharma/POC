using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanEligibilityService
    {
        AvailableFeature IsRiskEligibleForPlan(IRisk risk, IPlan plan, IEnumerable<CoverEligibilityResult> coverEligibilityResults);

        AvailableFeature IsRiskEligibleForPlan(IRisk risk, IPlan plan, IPlan parentPlan,
            IEnumerable<CoverEligibilityResult> coverEligibilityResults);
    }
    public class PlanEligibilityService : IPlanEligibilityService
    {
        private readonly IPlanEligibilityRulesFactory _planEligibilityRulesFactory;
        private readonly IPlanDefinitionProvider _planDefinitionProvider;
        private readonly IProductErrorMessageService _errorMessageService;
        private readonly IPlanMaxEntryAgeNextBirthdayProvider _planMaxEntryAgeNextBirthdayProvider;
        private readonly IMaxCoverAmountParamConverter _maxCoverAmountParamConverter;
        private readonly ICoverAmountService _coverAmountService;
        private readonly IProductBrandProvider _productBrandProvider;

        public PlanEligibilityService(IPlanEligibilityRulesFactory planEligibilityRulesFactory, IPlanDefinitionProvider planDefinitionProvider, IProductErrorMessageService errorMessageService, IPlanMaxEntryAgeNextBirthdayProvider planMaxEntryAgeNextBirthdayProvider, IMaxCoverAmountParamConverter maxCoverAmountParamConverter, ICoverAmountService coverAmountService, IProductBrandProvider productBrandProvider)
        {
            _planEligibilityRulesFactory = planEligibilityRulesFactory;
            _planDefinitionProvider = planDefinitionProvider;
            _errorMessageService = errorMessageService;
            _planMaxEntryAgeNextBirthdayProvider = planMaxEntryAgeNextBirthdayProvider;
            _maxCoverAmountParamConverter = maxCoverAmountParamConverter;
            _coverAmountService = coverAmountService;
            _productBrandProvider = productBrandProvider;
        }

        public AvailableFeature IsRiskEligibleForPlan(IRisk risk, IPlan plan,
            IEnumerable<CoverEligibilityResult> coverEligibilityResults)
        {
            return IsRiskEligibleForPlan(risk, plan, null, coverEligibilityResults);
        }

        public AvailableFeature IsRiskEligibleForPlan(IRisk risk, IPlan plan, IPlan parentPlan, IEnumerable<CoverEligibilityResult> coverEligibilityResults)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForRisk(risk);
            var planDefinition = _planDefinitionProvider.GetPlanByCode(plan.Code, brandKey);

            if (coverEligibilityResults.All(r => !r.EligibleForCover))
            {
                return AvailableFeature.Unavailable(plan.Code,
                    _errorMessageService.GetPlanHasNoValidOptionsErrorMessage());
            }

            var minAgeRule = _planEligibilityRulesFactory.GetMinmumAgeRule(planDefinition.MinimumEntryAgeNextBirthday);
            var minAgeRuleResult = minAgeRule.IsSatisfiedBy(risk.DateOfBirth);
            if (minAgeRuleResult.IsBroken)
            {
                return AvailableFeature.Unavailable(plan.Code,
                    _errorMessageService.GetMinimumAgeErrorMessage(risk.DateOfBirth.AgeNextBirthday()));
            }

            var maxEntryAge = _planMaxEntryAgeNextBirthdayProvider.GetMaxAgeFrom(planDefinition, risk.OccupationClass);

            var maxAgeRule = _planEligibilityRulesFactory.GetMaximumAgeRule(maxEntryAge);
            var maxAgeRuleResult = maxAgeRule.IsSatisfiedBy(risk.DateOfBirth);
            if (maxAgeRuleResult.IsBroken)
            {
                return AvailableFeature.Unavailable(plan.Code,
                    _errorMessageService.GetMaximumAgeErrorMessage(maxEntryAge));
            }

            var maxCoverAmountParam = _maxCoverAmountParamConverter.CreateFrom(risk, plan, parentPlan, brandKey);
            var maxCoverAmount = _coverAmountService.GetMaxCover(maxCoverAmountParam);
            var minCoverAmount = _coverAmountService.GetMinCover(plan.Code, brandKey);

            if (!_planEligibilityRulesFactory.GetMaxCoverAmountMustBeOverMinCoverAmountRule(planDefinition).IsSatisfiedBy(maxCoverAmount))
            {
                return AvailableFeature.Unavailable(plan.Code,
                       _errorMessageService.GetMinGreaterThanMaxCoverAmountErrorMessage(minCoverAmount, maxCoverAmount));
            }


            return AvailableFeature.Available(plan.Code);
        }
    }

}
