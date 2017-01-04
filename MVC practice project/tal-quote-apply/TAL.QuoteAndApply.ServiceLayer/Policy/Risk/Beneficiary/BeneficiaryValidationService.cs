using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Monads;
using System.Reflection;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Beneficiary;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Rules;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary
{
    public interface IBeneficiaryValidationService
    {
        ICollection<RiskBeneficiaryValidationModel> ValidateBeneficiariesForInforceForRisk(int riskId);

        ICollection<RiskBeneficiaryValidationModel> ValidateBeneficiariesForInForce(IEnumerable<IBeneficiary> beneficiaries, int riskId);

        ICollection<RiskBeneficiaryValidationModel> ValidateBeneficiariesForSave(IEnumerable<IBeneficiary> beneficiaries);
    }

    public class BeneficiaryValidationService : IBeneficiaryValidationService
    {
        private readonly IRiskService _riskService;
        private readonly IBenefeciaryRuleFactory _benefeciaryRuleFactory;

        public BeneficiaryValidationService(IRiskService riskService, IBenefeciaryRuleFactory benefeciaryRuleFactory)
        {
            _riskService = riskService;
            _benefeciaryRuleFactory = benefeciaryRuleFactory;
        }

        public ICollection<RiskBeneficiaryValidationModel> ValidateBeneficiariesForInforceForRisk(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            var beneficiaries = _riskService.GetBeneficiariesForRisk(risk);
            return ValidateBeneficiariesForInForce(beneficiaries, riskId);
        }

        private string GetPropertyNameOfBeneficiary(Expression<Func<IBeneficiary, object>> fieldSelector)
        {
            MemberExpression me;
            switch (fieldSelector.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = fieldSelector.Body as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = fieldSelector.Body as MemberExpression;
                    break;
            }

            return me.With(m => m.Member as PropertyInfo)
                    .With(propInfo => propInfo.Name.ToCamelCase());
        }

        private void UpdateValidationModel(RiskBeneficiaryValidationModel model, Func<RuleResult> funcRule, string message = null, bool messageOverride = false)
        {
            var ruleResult = funcRule.Invoke();
            if (!ruleResult.IsSatisfied)
            {
                if (string.IsNullOrEmpty(message))
                {
                    model.AddError(ruleResult);
                }
                else
                {
                    var msg = (messageOverride) ? message : ruleResult.Key.ToWords().ToTitleCase() + " " + message;                    
                    model.AddError(ruleResult, msg);
                }
            }
        }

        public ICollection<RiskBeneficiaryValidationModel> ValidateBeneficiariesForSave(
            IEnumerable<IBeneficiary> beneficiaries)
        {
            var beneficiariesArr = beneficiaries.ToArray();
            var retVal = beneficiariesArr.Select(x => new RiskBeneficiaryValidationModel(x.Id)).ToArray();

            for (var bIdx = 0; bIdx < beneficiariesArr.Length; bIdx++)
            {
                var beneficiary = beneficiariesArr[bIdx];
                var validationResult = retVal[bIdx];

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetBeneficiaryBenefitAmountRule(GetPropertyNameOfBeneficiary(b => b.Share)).IsSatisfiedBy(beneficiary.Share));

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetStringIsNumbersOnlyRule(GetPropertyNameOfBeneficiary(b => b.PhoneNumber))
                        .IsSatisfiedBy(beneficiary.PhoneNumber), "is not valid");

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetStringIsNumbersOnlyRule(GetPropertyNameOfBeneficiary(b => b.Postcode))
                        .IsSatisfiedBy(beneficiary.PhoneNumber), "is not valid");
            }

            return retVal;
        }

        public ICollection<RiskBeneficiaryValidationModel> ValidateBeneficiariesForInForce(IEnumerable<IBeneficiary> beneficiaries, int riskId)
        {
            var beneficiariesArr = beneficiaries.ToArray();
            var retVal = beneficiariesArr.Select(x => new RiskBeneficiaryValidationModel(x.Id)).ToList();

            var allBeneficiariesValidationModel = new RiskBeneficiaryValidationModel(0);
            var risk = _riskService.GetRisk(riskId);

            if (!risk.LprBeneficiary)
            {
                UpdateValidationModel(allBeneficiariesValidationModel,
                    () =>
                        _benefeciaryRuleFactory.GetAtLeastOneBeneficiaryRequiredRule("Beneficiaries")
                            .IsSatisfiedBy(beneficiariesArr),
                    "At least one beneficiary is required");
                retVal.Add(allBeneficiariesValidationModel);
            }

            for (var bIdx = 0; bIdx < beneficiariesArr.Length; bIdx++)
            {
                var beneficiary = beneficiariesArr[bIdx];
                var validationResult = retVal[bIdx];

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetSumOfBenefitShareRule(GetPropertyNameOfBeneficiary(b => b.Share)).IsSatisfiedBy(beneficiariesArr));

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetBeneficiaryBenefitAmountRule(GetPropertyNameOfBeneficiary(b => b.Share)).IsSatisfiedBy(beneficiary.Share));

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetTitleRequiredRule(GetPropertyNameOfBeneficiary(b => b.Title))
                        .IsSatisfiedBy(beneficiary.Title), "is required");

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetStringHasValueRule(GetPropertyNameOfBeneficiary(b => b.FirstName))
                        .IsSatisfiedBy(beneficiary.FirstName), "First name is required", true);

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetStringHasValueRule(GetPropertyNameOfBeneficiary(b => b.Surname))
                        .IsSatisfiedBy(beneficiary.Surname), "is required");

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetStringHasValueRule(GetPropertyNameOfBeneficiary(b => b.Address))
                        .IsSatisfiedBy(beneficiary.Address), "is required");

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetStringHasValueRule(GetPropertyNameOfBeneficiary(b => b.PhoneNumber))
                        .IsSatisfiedBy(beneficiary.PhoneNumber), "Phone number is required", true);

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetIsValidMobileOrLandlinePrefixRule(GetPropertyNameOfBeneficiary(b => b.PhoneNumber))
                        .IsSatisfiedBy(beneficiary.PhoneNumber), "is not valid");

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetStringIsBetweenMinAndMaxLengthRule(GetPropertyNameOfBeneficiary(b => b.Postcode), 4, 4)
                        .IsSatisfiedBy(beneficiary.Postcode), "must be 4 digits");

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetStringHasValueRule(GetPropertyNameOfBeneficiary(b => b.Postcode))
                        .IsSatisfiedBy(beneficiary.Postcode), "is required");

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetStringHasValueRule(GetPropertyNameOfBeneficiary(b => b.Suburb))
                        .IsSatisfiedBy(beneficiary.Suburb), "is required");

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetStateRuleRequiredRule(GetPropertyNameOfBeneficiary(b => b.State))
                        .IsSatisfiedBy(beneficiary.State), "is required");

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetDateOfBirthRule(GetPropertyNameOfBeneficiary(b => b.DateOfBirth))
                        .IsSatisfiedBy(beneficiary.DateOfBirth), "Date of birth is required", true);

                UpdateValidationModel(validationResult,
                    () => _benefeciaryRuleFactory.GetValidEmailAddressRule(GetPropertyNameOfBeneficiary(b => b.EmailAddress))
                        .IsSatisfiedBy(beneficiary.EmailAddress), "is not a valid email address");

                UpdateValidationModel(validationResult,
                    () =>
                        _benefeciaryRuleFactory.GetStringHasValueRule(
                            GetPropertyNameOfBeneficiary(b => b.BeneficiaryRelationshipId))
                            .IsSatisfiedBy(beneficiary.BeneficiaryRelationshipId.ToString()), "Relationship to the insured is required", true);
                
                UpdateValidationModel(validationResult,
                () => _benefeciaryRuleFactory.GetDateOfBirthMaxAgeRule(GetPropertyNameOfBeneficiary(b => b.DateOfBirth))
                    .IsSatisfiedBy(beneficiary.DateOfBirth));
            }

            return retVal;
        }

    }
}