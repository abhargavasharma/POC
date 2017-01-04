using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Cover
{
    public class SelectedCoverNotUnderwritingDeclinedRule : IRule<ICover>
    {
        public RuleResult IsSatisfiedBy(ICover target)
        {
            //Is underwriting declined, but only if it's selected
            if (!target.Selected)
            {
                return new RuleResult(true);
            }

            var coverNotDeclinedRule = new CoverNotUnderwritingDeclinedRule();
            return coverNotDeclinedRule.IsSatisfiedBy(target);
        }
    }
}
