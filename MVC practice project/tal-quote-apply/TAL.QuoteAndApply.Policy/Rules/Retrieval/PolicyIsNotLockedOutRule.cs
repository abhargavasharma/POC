using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Retrieval
{
    public class PolicyIsNotLockedOutRule : IRule<IPolicy>
    {
        public RuleResult IsSatisfiedBy(IPolicy target)
        {
            var lockedOut = target.SaveStatus == PolicySaveStatus.LockedOutDueToRefer;
            return new RuleResult(!lockedOut);
        }
    }
}