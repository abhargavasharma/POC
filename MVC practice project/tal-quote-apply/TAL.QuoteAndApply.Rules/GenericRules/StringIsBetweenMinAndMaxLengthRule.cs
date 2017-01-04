namespace TAL.QuoteAndApply.Rules.GenericRules
{
    public class StringIsBetweenMinAndMaxLengthRule : IRule<string>
    {
        private readonly string _validationKey;
        private readonly int _minLength;
        private readonly int _maxLength;

        public StringIsBetweenMinAndMaxLengthRule(string validationKey, int minLength, int maxLength)
        {
            _validationKey = validationKey;
            _minLength = minLength;
            _maxLength = maxLength;
        }

        public RuleResult IsSatisfiedBy(string target)
        {
            if (target == null || string.IsNullOrEmpty(target.ToString()))
            {
                return new RuleResult(_validationKey, true);
            }

            var length = target.ToString().Length;

            if (length < _minLength || length > _maxLength)
            {
                return new RuleResult(_validationKey, false);
            }

            return new RuleResult(_validationKey, true);
        }
    }
}