using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Risk
{
    public class ResidencyStatusRequiredRule : IRule<ResidencyStatus>
    {
        private readonly string _key;

        public ResidencyStatusRequiredRule(string key)
        {
            _key = key;
        }

        public RuleResult IsSatisfiedBy(ResidencyStatus residencyStatus)
        {
            return new RuleResult(_key, residencyStatus != ResidencyStatus.Unknown);
        }
    }
}