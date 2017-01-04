using System.Collections;
using System.Collections.Generic;

namespace TAL.QuoteAndApply.Rules.GenericRules
{
    public class IsValidMobilePrefixRule : IRule<string>
    {
        private readonly string _validationKey;

        public IsValidMobilePrefixRule(string validationKey)
        {
            _validationKey = validationKey;
        }

        public RuleResult IsSatisfiedBy(string target)
        {
            if (target == null)
            {
                return new RuleResult(_validationKey, true);
            }
            string numberStart = "04";
            var firstDigitsToCheck = target.ToString().Length < 2 ? target.ToString().Length : 2;
            var isSatisfied = (numberStart).Contains(target.ToString().Substring(0, firstDigitsToCheck));

            return new RuleResult(_validationKey, isSatisfied);
        }
    }
}
