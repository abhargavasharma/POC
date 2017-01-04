using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Cover
{
    public class CoverNotUnderwritingDeclinedRule : IRule<ICover>
    {
        public RuleResult IsSatisfiedBy(ICover target)
        {
            //Is underwriting declined, regardless of whether it's selected or not
            var isSatisfied = target.UnderwritingStatus != UnderwritingStatus.Decline;
            return new RuleResult(isSatisfied);
        }
    }
}
