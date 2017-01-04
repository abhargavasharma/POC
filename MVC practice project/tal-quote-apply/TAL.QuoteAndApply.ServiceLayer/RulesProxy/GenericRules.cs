using System;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.ServiceLayer.RulesProxy
{
    public interface IGenericRules
    {
        bool StringIsOnlyLettersSpacesHyphensRule(string val);
        bool StringIsNumbersOnlyRule(string val);
        bool StartsWithPhoneAreaCodeRule(string val);
        bool IsValidMobilePrefixRule(string val);
        bool IsValidMobileOrLandlinePrefixRuleValidation(string val);
        bool StringIsBetweenMinAndMaxLength(string val, int minLength, int maxLength);
        bool StringIsValidEmailAddress(string val);
        bool StringIsValidPasswordStrength(string val);
    }

    public class GenericRules : IGenericRules
    {
        private readonly IGenericRuleFactory _genericRuleFactory;

        public GenericRules(IGenericRuleFactory genericRuleFactory)
        {
            _genericRuleFactory = genericRuleFactory;
        }

        public bool StringIsOnlyLettersSpacesHyphensRule(string val)
        {
            var rule = _genericRuleFactory.GetStringIsOnlyLettersSpacesHyphensRule(String.Empty);

            return rule.IsSatisfiedBy(val).IsSatisfied;
        }

        public bool StringIsNumbersOnlyRule(string val)
        {
            var rule = _genericRuleFactory.GetStringIsNumbersOnlyRule(String.Empty);

            return rule.IsSatisfiedBy(val).IsSatisfied;
        }

        public bool StartsWithPhoneAreaCodeRule(string val)
        {
            var rule = _genericRuleFactory.GetStartsWithPhoneAreaCodeRule(String.Empty);

            return rule.IsSatisfiedBy(val).IsSatisfied;
        }
        public bool IsValidMobilePrefixRule(string val)
        {
            var rule = _genericRuleFactory.GetIsValidMobilePrefixRule(String.Empty);

            return rule.IsSatisfiedBy(val).IsSatisfied;
        }
        
        public bool IsValidMobileOrLandlinePrefixRuleValidation(string val)
        {
            var rule = _genericRuleFactory.GetIsValidMobileOrLandlinePrefixRule(String.Empty);

            return rule.IsSatisfiedBy(val).IsSatisfied;
        }

        public bool StringIsBetweenMinAndMaxLength(string val, int minLength, int maxLength)
        {
            var rule = _genericRuleFactory.GetStringIsBetweenMinAndMaxLengthRule(String.Empty, minLength, maxLength);

            return rule.IsSatisfiedBy(val).IsSatisfied;
        }

        public bool StringIsValidEmailAddress(string val)
        {
            var rule = _genericRuleFactory.GetValidEmailAddressRule(String.Empty);

            return rule.IsSatisfiedBy(val).IsSatisfied;
        }

        public bool StringIsValidPasswordStrength(string val)
        {
            var rules = _genericRuleFactory.GetStringIsValidPasswordStrengthRules(String.Empty);
            return rules.AllAreSatisfied(val).IsSatisfied;
        }
    }
}
