using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Retrieval
{
    public class PolicyIsSavedRule : IRule<IPolicy>
    {
        public RuleResult IsSatisfiedBy(IPolicy target)
        {
            var isSaved = target.SaveStatus == PolicySaveStatus.CreatedLogin 
                || target.SaveStatus == PolicySaveStatus.LockedOutDueToRefer;
            return new RuleResult(isSaved);
        }
    }
}
