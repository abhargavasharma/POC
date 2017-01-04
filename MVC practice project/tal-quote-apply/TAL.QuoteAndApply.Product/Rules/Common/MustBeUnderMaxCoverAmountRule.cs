using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Product.Rules.Common
{
    public class MustBeUnderMaxCoverAmountRule : IRule<int>
    {
        private readonly ICoverAmountService _coverAmountService;
        private readonly MaxCoverAmountParam _maxCoverParam;

        public MustBeUnderMaxCoverAmountRule(ICoverAmountService coverAmountService, MaxCoverAmountParam maxCoverParam)
        {
            _coverAmountService = coverAmountService;
            _maxCoverParam = maxCoverParam;
        }

        public RuleResult IsSatisfiedBy(int coverAmount)
        {

            var maxCover = _coverAmountService.GetMaxCover(_maxCoverParam);
            if (maxCover == 0)
            {
                return new RuleResult(true);
            }
            var overMaxCover = coverAmount > maxCover;
            return new RuleResult(!overMaxCover);
        }
    }
}
