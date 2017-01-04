using System.Text.RegularExpressions;

namespace TAL.QuoteAndApply.Rules.GenericRules
{
    public class ValidEmailAddressRule : IRule<string>
    {
        private readonly string _validationKey;

        private static Regex regexRule = new Regex(@"^[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ValidEmailAddressRule(string validationKey)
        {
            _validationKey = validationKey;
        }

        public RuleResult IsSatisfiedBy(string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return new RuleResult(_validationKey, true);
            }

            var isEmail = regexRule.IsMatch(target);

            return new RuleResult(_validationKey, isEmail);
        }
    }
}
