using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Risk
{
    public class SmokerStatusRequiredRule : IRule<SmokerStatus>
    {
        private readonly string _key;

        public SmokerStatusRequiredRule(string key)
        {
            _key = key;
        }


        public RuleResult IsSatisfiedBy(SmokerStatus smokerStatus)
        {
            return new RuleResult(_key, smokerStatus != SmokerStatus.Unknown);
        }
    }
}
