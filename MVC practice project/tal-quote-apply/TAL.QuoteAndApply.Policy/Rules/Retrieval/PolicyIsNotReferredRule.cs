using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Retrieval
{
    public class PolicyIsNotReferredRule : IRule<IPolicy>
    {
        public RuleResult IsSatisfiedBy(IPolicy target)
        {
            var notReferred = target.Status != PolicyStatus.ReferredToUnderwriter;
            return new RuleResult(notReferred);
        }
    }
}
