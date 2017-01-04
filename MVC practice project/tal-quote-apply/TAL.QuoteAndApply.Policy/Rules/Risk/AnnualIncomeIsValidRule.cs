using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Risk
{
    public class AnnualIncomeIsValidRule : IRule<long>
    {
        private readonly RiskProductDefinition _riskProductDefinition;
        private readonly string _key;


        public AnnualIncomeIsValidRule(RiskProductDefinition riskProductDefinition, string key)
        {
            _riskProductDefinition = riskProductDefinition;
            _key = key;
        }

        public RuleResult IsSatisfiedBy(long annualIncome)
        {

            if (!_riskProductDefinition.MinimumAnnualIncomeDollars.HasValue)
            {
                return new RuleResult(true);
            }

            var underMinAnnualIncome = annualIncome < _riskProductDefinition.MinimumAnnualIncomeDollars.Value;
            return new RuleResult(_key, !underMinAnnualIncome);
        }
    }
}
