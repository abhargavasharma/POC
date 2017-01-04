using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Cover;
using TAL.QuoteAndApply.Policy.Rules.Plan;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Rules;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Rules;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanRulesService
    {
        IEnumerable<ValidationError> ValidatePlanStateParam(PlanStateParam planStateParam);
        IEnumerable<ValidationError> ValidatePlanPremiumType(PremiumType premiumType, DateTime dateOfBirth, string brandKey);
    }

    public class PlanRulesService : IPlanRulesService
    {
        private readonly IPlanService _plansService;
        private readonly ICoverService _coverService;
        private readonly ICoverRulesFactory _coverRulesFactory;
        private readonly IPlanErrorMessageService _planErrorMessageService;
        private readonly IProductErrorMessageService _productErrorMessageService;
        private readonly IPlanDefinitionProvider _planDefinitionProvider;
        private readonly INameLookupService _nameLookupService;
        private readonly IRuleFactory _productRuleFactory;
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly IRiskService _riskService;
        private readonly IPlanRulesFactory _planRulesFactory;


        public PlanRulesService(IPlanService plansService, 
            ICoverService coverService, 
            ICoverRulesFactory coverRulesFactory, 
            IPlanErrorMessageService planErrorMessageService, 
            INameLookupService nameLookupService,
            IRuleFactory productRuleFactory,
            IProductDefinitionProvider productDefinitionProvider,
            IRiskService riskService, IProductErrorMessageService productErrorMessageService,
            IPlanDefinitionProvider planDefinitionProvider, IPlanRulesFactory planRulesFactory)
        {
            _plansService = plansService;
            _coverService = coverService;
            _coverRulesFactory = coverRulesFactory;
            _planErrorMessageService = planErrorMessageService;
            _nameLookupService = nameLookupService;
            _productRuleFactory = productRuleFactory;
            _productDefinitionProvider = productDefinitionProvider;
            _riskService = riskService;
            _productErrorMessageService = productErrorMessageService;
            _planDefinitionProvider = planDefinitionProvider;
            _planRulesFactory = planRulesFactory;
        }

        public IEnumerable<ValidationError> ValidatePlanStateParam(PlanStateParam planStateParam)
        {
            var errors = new List<ValidationError>();
            var selectedPlan = _plansService.GetByRiskIdAndPlanCode(planStateParam.RiskId, planStateParam.PlanCode);

            errors.AddRange(ValidatePlan(selectedPlan, planStateParam));
            errors.AddRange(ValidateCovers(selectedPlan, planStateParam.SelectedCoverCodes, planStateParam.BrandKey));
            
            //todo: need to validate riders as well

            return errors;
        }

        private IEnumerable<ValidationError> ValidatePlan(IPlan selectedPlan, PlanStateParam planStateParam)
        {
            var risk = _riskService.GetRisk(selectedPlan.RiskId);

            var errors = new List<ValidationError>();
            errors.AddRange(ValidateCoverSelections(planStateParam));
            errors.AddRange(ValidatePlanPremiumType(planStateParam.PremiumType, risk.DateOfBirth, planStateParam.BrandKey));
            errors.AddRange(ValidateCoverAmount(planStateParam));
            errors.AddRange(ValidatePlanVariables(planStateParam));
            errors.AddRange(ValidatePlanOptions(planStateParam));
            return errors;
        }

        private IEnumerable<ValidationError> ValidateCoverSelections(PlanStateParam planStateParam)
        {
            var errors = new List<ValidationError>();

            if (planStateParam.Selected && !_productRuleFactory.GetSelectedPlanMustHaveAtLeastCoverSelected().IsSatisfiedBy(planStateParam.SelectedCoverCodes))
            {
                errors.Add(new ValidationError(planStateParam.PlanCode, ValidationKey.InvalidPlanSelection,
                        _planErrorMessageService.GetPlanNeedsCoverSelectedErrorMessage(_nameLookupService.GetPlanName(planStateParam.PlanCode, planStateParam.BrandKey)),
                        ValidationType.Warning, planStateParam.PlanCode));
            }

            foreach (var rider in planStateParam.Riders)
            {
                if (rider.Selected && !_productRuleFactory.GetSelectedPlanMustHaveAtLeastCoverSelected().IsSatisfiedBy(rider.SelectedCoverCodes))
                {
                    errors.Add(new ValidationError(rider.PlanCode, ValidationKey.InvalidRiderSelection,
                            _planErrorMessageService.GetPlanNeedsCoverSelectedErrorMessage(_nameLookupService.GetPlanName(rider.PlanCode, planStateParam.BrandKey)),
                            ValidationType.Warning, rider.PlanCode));
                }
            }

            return errors;
        }

        private IEnumerable<ValidationError> ValidateCoverAmount(PlanStateParam planStateParam)
        {
            var errors = new List<ValidationError>();
            if (!_productRuleFactory.GetMustHaveCoverAmountRule().IsSatisfiedBy(planStateParam.CoverAmount))
            {
                errors.Add(new ValidationError("CoverAmount", ValidationKey.RequiredPlanCoverAmount,
                        _productErrorMessageService.GetSumInsuredRequiredErrorMessage(),
                        ValidationType.Error, planStateParam.PlanCode));
            }

            return errors;
        }

        private IEnumerable<ValidationError> ValidatePlanOptions(PlanStateParam planStateParam)
        {
            var errors = new List<ValidationError>();

            foreach (var o in planStateParam.PlanOptions)
            {
                if (!_productRuleFactory.GetFieldIsRequiredRule().AllAreSatisfied((object)o.Selected))
                {
                    errors.Add(new ValidationError(o.Code, ValidationKey.RequiredPlanOption,
                            _productErrorMessageService.GetSelectionRequiredErrorMessage(_nameLookupService.GetOptionName(planStateParam.PlanCode, o.Code, planStateParam.BrandKey)),
                            ValidationType.Warning, planStateParam.PlanCode));
                }
            }

            return errors;
        }

        private IEnumerable<ValidationError> ValidatePlanVariables(PlanStateParam planStateParam)
        {
            var errors = new List<ValidationError>();

            var planDefinition = _planDefinitionProvider.GetPlanByCode(planStateParam.PlanCode, planStateParam.BrandKey);

            foreach (var variable in planDefinition.Variables)
            {
                if (variable.Code == ProductPlanVariableConstants.LinkedToCpi)
                {
                    var linkedToCpi = (object)planStateParam.LinkedToCpi;
                    if (!_productRuleFactory.GetFieldIsRequiredRule().AllAreSatisfied(linkedToCpi))
                    {
                        errors.Add(GetPlanVariableRequiredValidationError(planStateParam.PlanCode, variable.Code, planStateParam.BrandKey));
                    }

                    if (!_productRuleFactory.GetVariableOptionMustBeValidRule(variable).AllAreSatisfied(linkedToCpi))
                    {
                        errors.Add(GetPlanVariableValueValidationError(planStateParam.PlanCode, planStateParam.BrandKey, variable.Code, linkedToCpi.ToString()));
                    }
                }
                if (variable.Code == ProductPlanVariableConstants.PremiumHoliday)
                {
                    var premiumHoliday = (object)planStateParam.PremiumHoliday;
                    if (!_productRuleFactory.GetFieldIsRequiredRule().AllAreSatisfied(premiumHoliday))
                    {
                        errors.Add(GetPlanVariableRequiredValidationError(planStateParam.PlanCode, variable.Code, planStateParam.BrandKey));
                    }
                    if (!_productRuleFactory.GetVariableOptionMustBeValidRule(variable).AllAreSatisfied(premiumHoliday))
                    {
                        errors.Add(GetPlanVariableValueValidationError(planStateParam.PlanCode, planStateParam.BrandKey, variable.Code, premiumHoliday.ToString()));
                    }
                }
                if (variable.Code == ProductPlanVariableConstants.BenefitPeriod)
                {
                    var benefitPeriod = (object)planStateParam.BenefitPeriod;
                    if (!_productRuleFactory.GetFieldIsRequiredRule().AllAreSatisfied(benefitPeriod))
                    {
                        errors.Add(GetPlanVariableRequiredValidationError(planStateParam.PlanCode, variable.Code, planStateParam.BrandKey));
                    }

                    if (!_productRuleFactory.GetVariableOptionMustBeValidRule(variable).AllAreSatisfied(benefitPeriod))
                    {
                        errors.Add(GetPlanVariableValueValidationError(planStateParam.PlanCode, planStateParam.BrandKey, variable.Code, benefitPeriod.ToString()));
                    }
                }
                if (variable.Code == ProductPlanVariableConstants.WaitingPeriod)
                {
                    var waitingPeriod = (object)planStateParam.WaitingPeriod;

                    if (!_productRuleFactory.GetFieldIsRequiredRule().AllAreSatisfied(waitingPeriod))
                    {
                        errors.Add(GetPlanVariableRequiredValidationError(planStateParam.PlanCode, variable.Code, planStateParam.BrandKey));
                    }
                    if (!_productRuleFactory.GetVariableOptionMustBeValidRule(variable).AllAreSatisfied(waitingPeriod))
                    {
                        errors.Add(GetPlanVariableValueValidationError(planStateParam.PlanCode, planStateParam.BrandKey, variable.Code, waitingPeriod.ToString()));
                    }
                }
                if (variable.Code == ProductPlanVariableConstants.PremiumType)
                {
                    var premiumType = planStateParam.PremiumType;
                    if (!_productRuleFactory.GetPremiumTypeIsNotUnkownRule().AllAreSatisfied(premiumType))
                    {
                        errors.Add(GetPlanVariableRequiredValidationError(planStateParam.PlanCode, variable.Code, planStateParam.BrandKey));
                    }
                    if (!_productRuleFactory.GetVariableOptionMustBeValidRule(variable).AllAreSatisfied((object)premiumType))
                    {
                        errors.Add(GetPlanVariableValueValidationError(planStateParam.PlanCode, planStateParam.BrandKey, variable.Code, premiumType.ToString()));
                    }
                }
                if (variable.Code == ProductPlanVariableConstants.OccupationDefinition)
                {
                    var occupationDefinition = planStateParam.OccupationDefinition;
                    if (!_productRuleFactory.GetVariableOptionMustBeValidRule(variable).AllAreSatisfied((object)occupationDefinition))
                    {
                        errors.Add(GetPlanVariableValueValidationError(planStateParam.PlanCode, planStateParam.BrandKey, variable.Code, occupationDefinition.ToString()));
                    }

                    var risk = _riskService.GetRisk(planStateParam.RiskId);
                    if (!_planRulesFactory.GetOccupationDefinitionIsAllowedRule(risk).AllAreSatisfied(occupationDefinition))
                    {
                        errors.Add(GetPlanVariableValueValidationError(planStateParam.PlanCode, planStateParam.BrandKey, variable.Code, occupationDefinition.ToString()));
                    }
                }
            }

            return errors;
        }

        private ValidationError GetPlanVariableRequiredValidationError(string planCode, string variableCode, string brandKey)
        {
            return new ValidationError(variableCode, ValidationKey.RequiredPlanVariable,
                            _productErrorMessageService.GetSelectionRequiredErrorMessage(_nameLookupService.GetVariableName(planCode, variableCode, brandKey)),
                            ValidationType.Warning, planCode);
        }

        private ValidationError GetPlanVariableValueValidationError(string planCode, string brandKey, string variableCode, string invalidValue)
        {
            return new ValidationError(variableCode, ValidationKey.PlanVariableInvalid,
                            _productErrorMessageService.GetInvalidVariableValueErrorMessage(_nameLookupService.GetVariableName(planCode, variableCode, brandKey), invalidValue),
                            ValidationType.Error, planCode);
        }

        public IEnumerable<ValidationError> ValidatePlanPremiumType(PremiumType premiumType, DateTime dateOfBirth, string brandKey)
        {
            var errors = new List<ValidationError>();
            
            var premiumTypeDefinition = _productDefinitionProvider.GetPremiumTypeDefinition(premiumType, brandKey);
            var premiumTypeRule = _productRuleFactory.GetEligibleForPremiumTypeRules(premiumTypeDefinition);

            var premiumTypeRuleOutcome = premiumTypeRule.AllAreSatisfied(dateOfBirth);
            if (premiumTypeRuleOutcome.IsBroken)
            {
                errors.Add(new ValidationError(null, 
                    ValidationKey.EligiblePremiumType,
                    _planErrorMessageService.GetPremiumTypeNotAvailableMessage(premiumTypeDefinition.PremiumType,
                        premiumTypeDefinition.MaximumEntryAgeNextBirthday.Value), ValidationType.Error, null));
            }

            return errors;
        }

        private IEnumerable<ValidationError> ValidateCovers(IPlan plan, IEnumerable<string> selectedCovers, string brandKey)
        {
            var rule = _coverRulesFactory.GetSelectedCoverNotUnderwritingDeclinedRule();

            var errors = new List<ValidationError>();

            var allCovers = _coverService.GetCoversForPlan(plan.Id);
            foreach (var cover in allCovers)
            {
                cover.Selected = selectedCovers.Contains(cover.Code);
                var ruleOutcome = rule.IsSatisfiedBy(cover);
                if (ruleOutcome.IsBroken)
                {
                    var errorCode = _planErrorMessageService.GetCoverErrorMessageCode(plan.Code, cover.Code,
                        _nameLookupService.GetCoverName(plan.Code, cover.Code, brandKey));

                    errors.Add(new ValidationError(errorCode,
                        ValidationKey.SelectedCoverUnderwritingDeclined,
                        _planErrorMessageService.GetSelectedCoverUndwritingDeclinedMessage(), ValidationType.Error, plan.Code));
                }
            }

            return errors;
        }
    }
}
