using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Plan
{
    public interface IOccupationDefinitionIsAllowedRule : IRule<OccupationDefinition>
    {
    }

    public class OccupationDefinitionIsAllowedRule : IOccupationDefinitionIsAllowedRule
    {
        private readonly IRisk _risk;

        public OccupationDefinitionIsAllowedRule(IRisk risk)
        {
            _risk = risk;
        }

        public RuleResult IsSatisfiedBy(OccupationDefinition target)
        {
            if (!_risk.IsTpdAny && target == OccupationDefinition.AnyOccupation)
            {
                return RuleResult.NotSatisfied("Any Occupation is not available for the current Occupation");
            }
            if (!_risk.IsTpdOwn && target == OccupationDefinition.OwnOccupation)
            {
                return RuleResult.NotSatisfied("Own Occupation is not available for the current Occupation");
            }
            return RuleResult.ToResult(true);
        }
    }
}