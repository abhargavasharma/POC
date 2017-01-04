using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Risk
{

    public class AustralianResidencyRule : IRule<ResidencyStatus>
    {
        private readonly RiskProductDefinition _riskProductDefinition;
        private readonly string _key;

        public AustralianResidencyRule(RiskProductDefinition riskProductDefinition, string key)
        {
            _riskProductDefinition = riskProductDefinition;
            _key = key;
        }

        public RuleResult IsSatisfiedBy(ResidencyStatus residencyStatus)
        {
            if (!_riskProductDefinition.AustralianResidencyRequired)
            {
                return new RuleResult(true);
            }
            
            return new RuleResult(_key, residencyStatus == ResidencyStatus.Australian);
        }
    }
}
