using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Risk
{
    public class AnnualIncomeRequiredRule : IRule<long>
    {
        private readonly string _key;

        public AnnualIncomeRequiredRule(string key)
        {
            _key = key;
        }

        public RuleResult IsSatisfiedBy(long annualIncome)
        {
            return new RuleResult(_key, annualIncome > 0);
        }
    }
}