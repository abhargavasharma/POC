using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Product.Rules.Common
{
    public class SelectedPlanMustHaveAtLeastCoverSelected : IRule<IEnumerable<string>>
    {
        public RuleResult IsSatisfiedBy(IEnumerable<string> target)
        {
            var result = target != null && target.Any();
            return new RuleResult(result);
        }
    }
}