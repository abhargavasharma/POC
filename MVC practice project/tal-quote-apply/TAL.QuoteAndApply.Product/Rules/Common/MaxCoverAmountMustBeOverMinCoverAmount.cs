using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Product.Rules.Common
{
    public class MaxCoverAmountMustBeOverMinCoverAmount : IRule<int>
    {
        private readonly PlanDefinition _planDefinition;

        public MaxCoverAmountMustBeOverMinCoverAmount(PlanDefinition planDefinition)
        {
            _planDefinition = planDefinition;
        }

        public RuleResult IsSatisfiedBy(int maxCoverAmount)
        {
            var maxUnderMinCover = maxCoverAmount < _planDefinition.MinimumCoverAmount;
            return new RuleResult(!maxUnderMinCover);
        }
    }
}
