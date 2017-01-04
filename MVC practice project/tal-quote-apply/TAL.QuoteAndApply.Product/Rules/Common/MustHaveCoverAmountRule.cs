using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Product.Rules.Common
{
    public class MustHaveCoverAmountRule : IRule<decimal>
    {
        public RuleResult IsSatisfiedBy(decimal target)
        {
            return new RuleResult(target > 0);
        }
    }
}
