using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Beneficiary
{
    public class BeneficiaryBenefitAmountRule : IRule<float>
    {
        private readonly string _key;

        public BeneficiaryBenefitAmountRule(string key)
        {
            _key = key;
        }

        public RuleResult IsSatisfiedBy(float share)
        {
            if (share < 1)
            {
                return RuleResult.ToResult(_key, false, "Benefit must not be less than 1%");
            }
            return RuleResult.ToResult(true);
        }
    }
}