using System.Collections;

namespace TAL.QuoteAndApply.Rules.GenericRules
{
    public class StartsWithPhoneAreaCodeRule : IRule<string>
    {
        private readonly string _validationKey;

        public StartsWithPhoneAreaCodeRule(string validationKey)
        {
            _validationKey = validationKey;
        }

        public RuleResult IsSatisfiedBy(string target)
        {
            if (target == null)
            {
                return new RuleResult(_validationKey, true);
            }

            string[] array = { "02", "03", "07", "08" };
            var firstDigitsToCheck = target.ToString().Length < 2 ? target.ToString().Length : 2;
            var isSatisfied = ((IList) array).Contains(target.ToString().Substring(0, firstDigitsToCheck));

            return new RuleResult(_validationKey, isSatisfied);
        }
    }
}