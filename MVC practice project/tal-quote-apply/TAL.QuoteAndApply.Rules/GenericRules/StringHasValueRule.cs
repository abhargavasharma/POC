namespace TAL.QuoteAndApply.Rules.GenericRules
{
    public class StringHasValueRule : IRule<string>
    {
        private readonly string _validationKey;

        public StringHasValueRule(string validationKey)
        {
            _validationKey = validationKey;
        }

        public RuleResult IsSatisfiedBy(string target)
        {
            var isSatisfied = !string.IsNullOrWhiteSpace(target);

            return new RuleResult(_validationKey, isSatisfied);
        }
    }
}
