using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Product.Rules.Common
{
    public class PremiumTypeIsNotUnkownRule : IRule<PremiumType>
    {
        public RuleResult IsSatisfiedBy(PremiumType target)
        {
            return new RuleResult(target != PremiumType.Unknown);
        }
    }
}