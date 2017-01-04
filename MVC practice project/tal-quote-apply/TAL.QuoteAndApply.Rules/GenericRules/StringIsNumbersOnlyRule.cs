using System.Text.RegularExpressions;

namespace TAL.QuoteAndApply.Rules.GenericRules
{
    public class StringIsNumbersOnlyRule : IRule<string>
    {
        private readonly string _validationKey;

        public StringIsNumbersOnlyRule(string validationKey)
        {
            _validationKey = validationKey;
        }

        public RuleResult IsSatisfiedBy(string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return new RuleResult(_validationKey, true);
            }

            var isNumbersOnly = Regex.IsMatch(target, @"^\d+$");
            return new RuleResult(_validationKey, isNumbersOnly);
        }
    }
}
