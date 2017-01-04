using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Product.Rules.Common
{
    public class MustBeOverMinumumCoverAmountRule : IRule<int>
    {
        private readonly PlanDefinition _planDefinition;

        public MustBeOverMinumumCoverAmountRule(PlanDefinition planDefinition)
        {
            _planDefinition = planDefinition;
        }

        public RuleResult IsSatisfiedBy(int coverAmount)
        {
            var underMinCover = coverAmount < _planDefinition.MinimumCoverAmount;
            return new RuleResult(!underMinCover);
        }
    }
}
