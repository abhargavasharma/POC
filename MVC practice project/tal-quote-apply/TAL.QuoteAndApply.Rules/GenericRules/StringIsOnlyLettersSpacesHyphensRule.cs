using System.Text.RegularExpressions;

namespace TAL.QuoteAndApply.Rules.GenericRules
{
    public class StringIsOnlyLettersSpacesHyphensRule : IRule<string>
    {
        private readonly string _validationKey;

        public StringIsOnlyLettersSpacesHyphensRule(string validationKey)
        {
            _validationKey = validationKey;
        }

        public RuleResult IsSatisfiedBy(string target)
        {
            //allows empty string
            if (target == null || string.IsNullOrEmpty(target))
            {
                return new RuleResult(_validationKey, true);
            }

            var isSatisfied = Regex.IsMatch((string)target, @"^[ A-Za-z-']*$");
            return new RuleResult(_validationKey, isSatisfied);
        }
    }
}