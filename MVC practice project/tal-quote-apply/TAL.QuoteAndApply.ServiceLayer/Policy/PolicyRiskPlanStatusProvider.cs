using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Rules.Risk;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Rules;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyRiskPlanStatusProvider
    {
        PolicyRiskPlanStatusesResult GetFor(string quoteReference, int riskId);
    }

    public class PolicyRiskPlanStatusProvider : IPolicyRiskPlanStatusProvider
    {
        private readonly IPolicyWithRisksService _policyWithRisksService;
        private readonly IPlanService _planService;
        private readonly IActivePlanValidationService _activePlanValidationService;
        private readonly IPlanStateParamProvider _planStateParamProvider;
        private readonly IRiskRulesFactory _riskRulesFactory;

        public PolicyRiskPlanStatusProvider(IPolicyWithRisksService policyWithRisksService, IPlanService planService, IActivePlanValidationService activePlanValidationService, IPlanStateParamProvider planStateParamProvider, IRiskRulesFactory riskRulesFactory)
        {
            _policyWithRisksService = policyWithRisksService;
            _planService = planService;
            _activePlanValidationService = activePlanValidationService;
            _planStateParamProvider = planStateParamProvider;
            _riskRulesFactory = riskRulesFactory;
        }

        public PolicyRiskPlanStatusesResult GetFor(string quoteReference, int riskId)
        {
            var planValidationStatuses = new List<PlanValidationStatus>();

            var policyWithRisks = _policyWithRisksService.GetFrom(quoteReference);
            var riskWithPlans = policyWithRisks.Risks.First(r => r.Risk.Id == riskId);
            var allPlans = riskWithPlans.Plans.Select(p => p.Plan);

            var parentPlans = _planService.GetParentPlansFromAllPlans(allPlans);

            foreach (var plan in parentPlans.Where(x=> x.Selected))
            {
                var planStateParam = _planStateParamProvider.CreateFrom(riskWithPlans.Risk, plan, allPlans);

                var result = _activePlanValidationService.ValidateCurrentActivePlan(planStateParam);

                var errors = new List<ValidationError>();
                var warnings = new List<ValidationError>();

                if (result.HasErrors)
                {
                    errors.AddRange(result.Errors);
                }
                if (result.HasWarnings)
                {
                    warnings.AddRange(result.Warnings);
                }

                planValidationStatuses.Add(new PlanValidationStatus(plan.Code, GetPlanStatusFromErrorsAndWarnings(result), errors, warnings));
            }

            var riskRule = _riskRulesFactory.GetAtLeastOnePlanMustBeSelectedRule("RiskPlans");
            var riskRuleResult = riskRule.IsSatisfiedBy(allPlans);

            return new PolicyRiskPlanStatusesResult(GetOverallStatus(planValidationStatuses, riskRuleResult), planValidationStatuses);
        }

        private PlanStatus GetOverallStatus(IEnumerable<PlanValidationStatus> allPlans, RuleResult riskRuleResult)
        {
            var errors = allPlans.SelectMany(x => x.Errors);
            var warnings = allPlans.SelectMany(x => x.Warnings);

            if (riskRuleResult.IsBroken)
            {
                return PlanStatus.Warning;
            }

            if (errors.Any())
            {
                return PlanStatus.Error;
            }

            if (warnings.Any())
            {
                return PlanStatus.Warning;
            }

            return PlanStatus.Completed;
        }

        private PlanStatus GetPlanStatusFromErrorsAndWarnings(EditPolicyResult result)
        {
            if (result.HasErrors)
            {
                return PlanStatus.Error;
            }
            if (result.HasWarnings)
            {
                return PlanStatus.Warning;
            }
            return PlanStatus.Completed;
        }

    }
}
