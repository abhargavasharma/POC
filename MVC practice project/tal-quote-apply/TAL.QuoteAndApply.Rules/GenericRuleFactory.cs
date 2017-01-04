using System.Collections.Generic;
using TAL.QuoteAndApply.Rules.GenericRules;

namespace TAL.QuoteAndApply.Rules
{
    public interface IGenericRuleFactory
    {
        IRule<string> GetStringIsOnlyLettersSpacesHyphensRule(string validationKey);
        IRule<string> GetStringIsNumbersOnlyRule(string validationKey);
        IRule<string> GetStartsWithPhoneAreaCodeRule(string validationKey);
        IRule<string> GetIsValidMobilePrefixRule(string validationKey);
        IRule<string> GetIsValidMobileOrLandlinePrefixRule(string validationKey);
        IRule<string> GetStringIsBetweenMinAndMaxLengthRule(string validationKey, int minLength, int maxLength);

        IRule<string> GetStringHasValueRule(string validationKey);

        IRule<string> GetValidEmailAddressRule(string validationKey);
        IEnumerable<IRule<string>> GetStringIsValidPasswordStrengthRules(string validationKey);
    }

    public class GenericRuleFactory : IGenericRuleFactory
    {
        public IRule<string> GetStringIsOnlyLettersSpacesHyphensRule(string validationKey)
        {
            return new StringIsOnlyLettersSpacesHyphensRule(validationKey);
        }

        public IRule<string> GetStringIsNumbersOnlyRule(string validationKey)
        {
            return new StringIsNumbersOnlyRule(validationKey);
        }

        public IRule<string> GetStartsWithPhoneAreaCodeRule(string validationKey)
        {
            return new StartsWithPhoneAreaCodeRule(validationKey);
        }

        public IRule<string> GetIsValidMobilePrefixRule(string validationKey)
        {
            return new IsValidMobilePrefixRule(validationKey);
        }
        public IRule<string> GetIsValidMobileOrLandlinePrefixRule(string validationKey)
        {
            return new IsValidMobileOrLandlinePrefixRule(validationKey);
        }

        public IRule<string> GetStringIsBetweenMinAndMaxLengthRule(string validationKey, int minLength, int maxLength)
        {
            return new StringIsBetweenMinAndMaxLengthRule(validationKey, minLength, maxLength);
        }

        public IRule<string> GetStringHasValueRule(string validationKey)
        {
            return new StringHasValueRule(validationKey);
        }

        public IRule<string> GetValidEmailAddressRule(string validationKey)
        {
            return new ValidEmailAddressRule(validationKey);
        }

        public IEnumerable<IRule<string>> GetStringIsValidPasswordStrengthRules(string validationKey)
        {
            //Removed StringContainsAtLeastOneNumberRule() and StringContainsAtLeastOneSpecialCharacterRule() for password strength requirement change US20959
            yield return new StringContainsAtLeastOneUppercaseLetterRule();
            yield return new StringContainsAtLeastOneLowercaseLettersRule();
            yield return new StringIsBetweenMinAndMaxLengthRule(validationKey, 6, 20);
        }
    }
}