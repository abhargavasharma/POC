using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Models.Converters;
using TAL.QuoteAndApply.Policy.Rules.Risk;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Rules;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Rules;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Product.Rules
{
    public interface IProductRulesService
    {
        IEnumerable<ValidationError> ValidateRatingFactors(RatingFactorsParam ratingFactors, bool validateResidency);
        IEnumerable<ValidationError> ValidateCoverAmountForPlan(PlanStateParam planOptionsParam);
        IEnumerable<ValidationError> ValidatePlanStateParam(PlanStateParam createPlanParam);
        IEnumerable<ValidationError> ValidateSelectedPlanForInForce(RaisePolicyPlan raisePolicyPlan);
        IEnumerable<ValidationError> ValidateIncome(long income);
        IEnumerable<ValidationError> ValidateAge(DateTime dateOfBirth);
    }

    public class ProductRulesService : IProductRulesService
    {

        private readonly IRuleFactory _productRulesFactory;
        private readonly IRiskRulesFactory _riskRulesFactory;
        private readonly IRiskProductDefinitionConverter _riskProductDefinitionConverter;
        private readonly IProductDefinitionBuilder _productDefinitionBuilder;
        private readonly IPlanDefinitionProvider _planDefinitionProvider;
        private readonly IProductErrorMessageService _errorMessageService;
        private readonly IAvailablePlanOptionsProvider _availablePlanOptionsProvider;
        private readonly ICoverAmountService _coverAmountService;
        private readonly IMaxCoverAmountParamConverter _maxCoverAmountParamConverter;
        private readonly IResidencyStatusConverter _residencyStatusConverter;
        private readonly INameLookupService _nameLookupService;
        private readonly IRiskService _riskService;
        private readonly IPlanVariableResponseConverter _planVariableResponseConverter;
        private readonly ICurrentProductBrandProvider _currentProductBrandProvider;

        public ProductRulesService(IRuleFactory productRulesFactory, IRiskRulesFactory riskRulesFactory,
            IRiskProductDefinitionConverter riskProductDefinitionConverter,
            IProductDefinitionBuilder productDefinitionBuilder,
            IProductErrorMessageService errorMessageService, IPlanDefinitionProvider planDefinitionProvider,
            IAvailablePlanOptionsProvider availablePlanOptionsProvider, ICoverAmountService coverAmountService,
            IMaxCoverAmountParamConverter maxCoverAmountParamConverter,
            IResidencyStatusConverter residencyStatusConverter, INameLookupService nameLookupService,
            IRiskService riskService, IPlanVariableResponseConverter planVariableResponseConverter, ICurrentProductBrandProvider currentProductBrandProvider)
        {
            _productRulesFactory = productRulesFactory;
            _riskRulesFactory = riskRulesFactory;
            _riskProductDefinitionConverter = riskProductDefinitionConverter;
            _productDefinitionBuilder = productDefinitionBuilder;
            _errorMessageService = errorMessageService;
            _planDefinitionProvider = planDefinitionProvider;
            _availablePlanOptionsProvider = availablePlanOptionsProvider;
            _coverAmountService = coverAmountService;
            _maxCoverAmountParamConverter = maxCoverAmountParamConverter;
            _residencyStatusConverter = residencyStatusConverter;
            _nameLookupService = nameLookupService;
            _riskService = riskService;
            _planVariableResponseConverter = planVariableResponseConverter;
            _currentProductBrandProvider = currentProductBrandProvider;
        }

        public IEnumerable<ValidationError> ValidateRatingFactors(RatingFactorsParam ratingFactors, bool validateResidency)
        {
            var errors = new List<ValidationError>();

            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(currentBrand.BrandCode);
            var riskProductDefinition = _riskProductDefinitionConverter.CreateFrom(productDefinition);

            if (validateResidency)
            {
                var residencyRules = _riskRulesFactory.GetAustralianResidencyRules("", riskProductDefinition);

                if (
                    residencyRules.AllAreSatisfied(
                        _residencyStatusConverter.GetResidency(ratingFactors.IsAustralianResident)).IsBroken)
                {
                    errors.Add(new ValidationError(null, ValidationKey.AustralianResidency,
                        _errorMessageService.GetAustralianResidentErrorMessage(), ValidationType.Error, null));
                }
            }

            ValidateAge(ratingFactors.DateOfBirth, errors, riskProductDefinition);

            ValidateIncome(ratingFactors.Income, errors, riskProductDefinition, productDefinition);

            return errors;

        }

        public IEnumerable<ValidationError> ValidateIncome(long income)
        {
            var errors = new List<ValidationError>();

            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(currentBrand.BrandCode);
            var riskProductDefinition = _riskProductDefinitionConverter.CreateFrom(productDefinition);

            ValidateIncome(income, errors, riskProductDefinition, productDefinition);

            return errors;
        }

        public IEnumerable<ValidationError> ValidateAge(DateTime dateOfBirth)
        {
            var errors = new List<ValidationError>();

            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(currentBrand.BrandCode);
            var riskProductDefinition = _riskProductDefinitionConverter.CreateFrom(productDefinition);

            ValidateAge(dateOfBirth, errors, riskProductDefinition);

            return errors;
        }

        private void ValidateIncome(long income, List<ValidationError> errors, RiskProductDefinition riskProductDefinition, ProductDefinition productDefinition)
        {
            if (!_riskRulesFactory.GetAnnualIncomeRules("", riskProductDefinition).AllAreSatisfied(income))
            {
                errors.Add(new ValidationError(null,
                    ValidationKey.AnnualIncome,
                    _errorMessageService.GetAnnualIncomeErrorMessage(income, productDefinition.MinimumAnnualIncomeDollars), 
                    ValidationType.Error, null));
            }
        }

        private void ValidateAge(DateTime dateOfBirth, List<ValidationError> errors, RiskProductDefinition riskProductDefinition)
        {
            if (!_riskRulesFactory.GetMinimumAgeRules("", riskProductDefinition).AllAreSatisfied(dateOfBirth))
            {
                errors.Add(new ValidationError(null,
                    ValidationKey.MinimumAge,
                    _errorMessageService.GetMinimumAgeErrorMessage(riskProductDefinition.MinimumEntryAgeNextBirthday),
                    ValidationType.Error, null));
            }

            if (!_riskRulesFactory.GetMaximumAgeRules("", riskProductDefinition).AllAreSatisfied(dateOfBirth))
            {
                errors.Add(new ValidationError(null,
                    ValidationKey.MaximumAge,
                    _errorMessageService.GetMaximumAgeErrorMessage(riskProductDefinition.MaximumEntryAgeNextBirthday),
                    ValidationType.Error, null));
            }
        }

        public IEnumerable<ValidationError> ValidateCoverAmountForPlan(PlanStateParam planOptionsParam)
        {
            var errors = new List<ValidationError>();

            var planDefinition = _planDefinitionProvider.GetPlanByCode(planOptionsParam.PlanCode, planOptionsParam.BrandKey);
            var maxCoverAmountParam = _maxCoverAmountParamConverter.CreateFrom(planOptionsParam);

            //check the min cover amount is greater than max cover amount , if not, not elgible for cover
            if (!_productRulesFactory.GetMaxGreaterThanMinCoverAmountRule(planDefinition).AllAreSatisfied(planOptionsParam.MaxCoverAmount))
            {
                errors.Insert(0, new ValidationError(planOptionsParam.PlanCode, ValidationKey.MaxGreaterThanMinCoverAmount,
                    _errorMessageService.GetMinGreaterThanMaxCoverAmountErrorMessage(planDefinition.MinimumCoverAmount,
                    planOptionsParam.MaxCoverAmount), ValidationType.Error, planOptionsParam.PlanCode));

                return errors;
            }

            //check over min
            if (planOptionsParam.CoverAmount > 0 && !_productRulesFactory.GetMinCoverAmountRules(planDefinition).AllAreSatisfied(planOptionsParam.CoverAmount))
            {
                errors.Add(new ValidationError(planOptionsParam.PlanCode, ValidationKey.MinimumCoverAmount,
                    _errorMessageService.GetMinimumCoverAmountErrorMessage(planOptionsParam.Age, planOptionsParam.MinCoverAmount), ValidationType.Error, planOptionsParam.PlanCode));

                return errors;
            }

            //check not over max cover
            if (!_productRulesFactory.GetMaxCoverAmountRules(_coverAmountService, maxCoverAmountParam).AllAreSatisfied(planOptionsParam.CoverAmount))
            {
                errors.Add(new ValidationError(planOptionsParam.PlanCode, ValidationKey.MaximumCoverAmount,
                    _errorMessageService.GetMaximumCoverAmountErrorMessage(planOptionsParam.Age, planOptionsParam.MaxCoverAmount), ValidationType.Error, planOptionsParam.PlanCode));

                return errors;
            }

            return errors;
        }

        public IEnumerable<ValidationError> ValidatePlanStateParam(PlanStateParam planOptionsParam)
        {
            var errors = new List<ValidationError>();

            var risk = _riskService.GetRisk(planOptionsParam.RiskId);

            var planState = planOptionsParam.ToAvailabilityPlanStateParam(risk.OccupationClass);
            var availableOptions = _availablePlanOptionsProvider.GetForPlan(planState);
            var invalidSelectedPlans = planState.SelectedPlanCodes.Where(
                selectedPlan => !availableOptions.AvailablePlans.Contains(selectedPlan))
                .ToArray();

            const ValidationType reportErrorsAs = ValidationType.Warning;

            if (invalidSelectedPlans.Any())
            {
                foreach (var invalidSelectedPlan in invalidSelectedPlans)
                {
                    errors.Add(new ValidationError(invalidSelectedPlan, ValidationKey.InvalidPlanSelection,
                        _errorMessageService.GetInvalidPlanSelectedErrorMessage(invalidSelectedPlan,
                            availableOptions.UnavailableFeatures), reportErrorsAs, invalidSelectedPlan));
                }
            }

            var invalidSelectedCovers = planState.SelectedCoverCodes.Where(
                selectedPlan => !availableOptions.AvailableCovers.Contains(selectedPlan)).ToArray();

            if (invalidSelectedCovers.Any())
            {
                foreach (var invalidCover in invalidSelectedCovers)
                {
                    errors.Add(new ValidationError(invalidCover, ValidationKey.InvalidCoverSelection,
                        _errorMessageService.GetInvalidCoverSelectedErrorMessage(invalidCover,
                            availableOptions.UnavailableFeatures), reportErrorsAs, invalidCover));
                }
            }

            var invalidSelectedRiderCovers =
                planState.With(p => p.SelectedRiderCoverCodes).Return(q => planState.SelectedRiderCoverCodes.Where(
                    selectedPlan =>
                        !availableOptions.AvailableRiders.SelectMany(r => r.AvailableCovers).Contains(selectedPlan))
                    .ToArray(), null);

            if (invalidSelectedRiderCovers.IsNotNull() && invalidSelectedRiderCovers.Any())
            {
                foreach (var invalidRider in invalidSelectedRiderCovers)
                {
                    errors.Add(new ValidationError(invalidRider, ValidationKey.InvalidRiderCoverSelection,
                        _errorMessageService.GetInvalidRiderCoverSelectedErrorMessage(invalidRider,
                            availableOptions.UnavailableFeatures), reportErrorsAs, invalidRider));
                }
            }
            var selectedPlanOptions = planState.SelectedPlanOptionCodes.ToArray();
            var invalidSelectedPlanOptions =
                planState.With(p => p.SelectedPlanOptionCodes).Return(q => selectedPlanOptions.Where(
                    selectedPlan => !availableOptions.AvailableOptions.Contains(selectedPlan)).ToArray(), null);

            if (invalidSelectedPlanOptions.IsNotNull() && invalidSelectedPlanOptions.Any())
            {
                foreach (var invalidPlanOption in invalidSelectedPlanOptions)
                {
                    errors.Add(new ValidationError(invalidPlanOption, ValidationKey.InvalidOptionSelection,
                        _errorMessageService.GetInvalidPlanOptionsSelectedErrorMessage(invalidPlanOption,
                            availableOptions.UnavailableFeatures), reportErrorsAs, invalidPlanOption));
                }
            }

            var invalidSelectedRiders =
                planState.With(p => p.SelectedRiderCodes).Return(q => planState.SelectedRiderCodes.Where(
                    selectedRider => availableOptions.UnavailableFeatures.Any(x => x.Code == selectedRider)).ToArray(),
                    null);

            if (invalidSelectedRiders.IsNotNull() && invalidSelectedRiders.Any())
            {
                foreach (var invalidRider in invalidSelectedRiders)
                {
                    errors.Add(new ValidationError(invalidRider, ValidationKey.InvalidRiderSelection,
                        _errorMessageService.GetInvalidRiderSelectedErrorMessage(invalidRider,
                            availableOptions.UnavailableFeatures), reportErrorsAs, invalidRider));
                }
            }

            //Variable availability
            foreach (var variableAvailableFeature in availableOptions.VariableAvailability)
            {
                if (!variableAvailableFeature.IsAvailable)
                {
                    errors.Add(new ValidationError(variableAvailableFeature.Code, ValidationKey.PlanVariableInvalid,
                        variableAvailableFeature.ReasonIfUnavailable.First(), reportErrorsAs, planOptionsParam.PlanCode));
                }
                else
                {
                    //If selected variable option is not available then report variable as unavailable
                    var selectedVariableOptionValue = _planVariableResponseConverter.SelectedValueFrom(variableAvailableFeature,
                        planOptionsParam);

                    var variableOptionValueAvailability =
                        variableAvailableFeature.ChildAvailableFeatures.SingleOrDefault(a => a.Code == selectedVariableOptionValue.ToString());

                    if (variableOptionValueAvailability != null && !variableOptionValueAvailability.IsAvailable)
                    {
                        errors.Add(new ValidationError(variableAvailableFeature.Code, ValidationKey.PlanVariableInvalid,
                           variableOptionValueAvailability.ReasonIfUnavailable.First(), reportErrorsAs,
                           planOptionsParam.PlanCode));
                    }
                }
            }


            return errors;
        }

        public IEnumerable<ValidationError> ValidateSelectedPlanForInForce(RaisePolicyPlan raisePolicyPlan)
        {
            const string linkedToCpiConst = "linkedToCpi";
            const string premiumHolidayConst = "premiumHoliday";
            var errors = new List<ValidationError>();
            if (raisePolicyPlan.Selected)
            {
                if (!_productRulesFactory.GetFieldIsRequiredRule().AllAreSatisfied((object)raisePolicyPlan.LinkedToCpi))
                {
                    errors.Add(new ValidationError(linkedToCpiConst, ValidationKey.LinkedToCpiRequired,          
                        _errorMessageService.GetSelectionRequiredErrorMessage(_nameLookupService.GetVariableName(raisePolicyPlan.Code, linkedToCpiConst, raisePolicyPlan.BrandKey)), 
                        ValidationType.Error, raisePolicyPlan.Code));
                }
                if (!_productRulesFactory.GetFieldIsRequiredRule().AllAreSatisfied((object)raisePolicyPlan.PremiumHoliday))
                {
                    errors.Add(new ValidationError(premiumHolidayConst.ToCamelCase(), ValidationKey.PremiumHolidayRequired, 
                        _errorMessageService.GetSelectionRequiredErrorMessage(_nameLookupService.GetVariableName(raisePolicyPlan.Code, premiumHolidayConst, raisePolicyPlan.BrandKey)), 
                        ValidationType.Error, raisePolicyPlan.Code));
                }
                foreach (var option in raisePolicyPlan.Options)
                {
                    if (!_productRulesFactory.GetFieldIsRequiredRule().AllAreSatisfied((object)option.Selected)) { 
                        var optionName = _nameLookupService.GetOptionName(raisePolicyPlan.Code, option.Code, raisePolicyPlan.BrandKey);
                        errors.Add(new ValidationError(optionName.ToCamelCase(), ValidationKey.PremiumReliefOption, _errorMessageService.GetSelectionRequiredErrorMessage(optionName),
                        ValidationType.Error,
                        raisePolicyPlan.Code));
                    }
                }
            }

            return errors;
        }
    }
}
