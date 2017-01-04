using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public interface IActivePlanValidationService
    {
        EditPolicyResult ValidateCurrentActivePlan(PlanStateParam editPolicyOptionsParam);
        EditPolicyResult ValidateCurrentActivePlanForInForce(RaisePolicyPlan policyOptionsParam);
    }

    public class ActivePlanValidationService : IActivePlanValidationService
    {
        private readonly IPlanStateParamValidationService _planStateParamValidationService;
        private readonly IProductRulesService _productRulesService;
        private readonly ICoverAmountService _coverAmountService;
        private readonly IMaxCoverAmountParamConverter _maxCoverAmountParamConverter;
        private readonly IPlanEligibilityProvider _planEligibilityProvider;

        public ActivePlanValidationService(IProductRulesService productRulesService,
            ICoverAmountService maxCoverAmountService,
            IMaxCoverAmountParamConverter maxCoverAmountParamConverter,
            IPlanStateParamValidationService planStateParamValidationService, 
            IPlanEligibilityProvider planEligibilityProvider)
        {
            _productRulesService = productRulesService;
            _coverAmountService = maxCoverAmountService;
            _maxCoverAmountParamConverter = maxCoverAmountParamConverter;
            _planStateParamValidationService = planStateParamValidationService;
            _planEligibilityProvider = planEligibilityProvider;
        }

        public EditPolicyResult ValidateCurrentActivePlan(PlanStateParam policyOptionsParam)
        {
            List<ValidationError> planBrokenRules = new List<ValidationError>();

            var planEligibility = _planEligibilityProvider.GetPlanEligibilityFor(policyOptionsParam.RiskId,
                policyOptionsParam.PlanId);

            if (policyOptionsParam.AllPlans.Any(p => p.PlanCode == policyOptionsParam.PlanCode)
                && planEligibility.EligibleForPlan)
            {
                var maxCoverAmountParam = _maxCoverAmountParamConverter.CreateFrom(policyOptionsParam);
                policyOptionsParam.MaxCoverAmount = _coverAmountService.GetMaxCover(maxCoverAmountParam);
                policyOptionsParam.MinCoverAmount = _coverAmountService.GetMinCover(policyOptionsParam.PlanCode, policyOptionsParam.BrandKey);
                var brokenRules = _productRulesService.ValidateCoverAmountForPlan(policyOptionsParam);
                
                foreach (var rider in policyOptionsParam.Riders.Where(r => r.Selected))
                {
                    var maxCoverAmountParamRider = _maxCoverAmountParamConverter.CreateFrom(rider);
                    rider.MaxCoverAmount = _coverAmountService.GetMaxCover(maxCoverAmountParamRider);
                    rider.MinCoverAmount = _coverAmountService.GetMinCover(rider.PlanCode, rider.BrandKey);
                    planBrokenRules.AddRange(_productRulesService.ValidateCoverAmountForPlan(rider));
                }

                planBrokenRules.AddRange(_planStateParamValidationService.ValidatePlanStateParam(policyOptionsParam));
                planBrokenRules.AddRange(brokenRules);

                if (planBrokenRules.Any(x => x.Type == ValidationType.Error))
                {
                    return new EditPolicyResult(planBrokenRules.Where(x => x.Type == ValidationType.Error), planEligibility.EligibleForPlan);
                }
            }
            return new EditPolicyResult(policyOptionsParam.MaxCoverAmount, policyOptionsParam.MinCoverAmount,
                policyOptionsParam.CoverAmount, planBrokenRules, planEligibility.EligibleForPlan );
        }

        public EditPolicyResult ValidateCurrentActivePlanForInForce(RaisePolicyPlan raisePolicyPlan)
        {
                //todo: this should maybe move somewhere else
            var brokenRules = _productRulesService.ValidateSelectedPlanForInForce(raisePolicyPlan);

            // Defaulting plan eligibility to True
            var validatedCurrentActivePlan = new EditPolicyResult(brokenRules, true);

            return validatedCurrentActivePlan;
        }
    }
}
