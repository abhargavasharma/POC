namespace TAL.QuoteAndApply.Rules.GenericRules
{
    public class IsValidMobileOrLandlinePrefixRule : IRule<string>
    {
        private readonly string _validationKey;

        public IsValidMobileOrLandlinePrefixRule(string validationKey)
        {
            _validationKey = validationKey;
        }

        public RuleResult IsSatisfiedBy(string target)
        {
            if (target == null)
            {
                return new RuleResult(_validationKey, true);
            }
            IsValidMobilePrefixRule isValidMobilePrefixRule = new IsValidMobilePrefixRule(_validationKey);
            StartsWithPhoneAreaCodeRule startsWithPhoneAreaCodeRule = new StartsWithPhoneAreaCodeRule(_validationKey);
            var isSatisfied = ((isValidMobilePrefixRule.IsSatisfiedBy(target).IsSatisfied) ||
                (startsWithPhoneAreaCodeRule.IsSatisfiedBy(target).IsSatisfied)) ;

            return new RuleResult(_validationKey, isSatisfied);
        }
    }
}
