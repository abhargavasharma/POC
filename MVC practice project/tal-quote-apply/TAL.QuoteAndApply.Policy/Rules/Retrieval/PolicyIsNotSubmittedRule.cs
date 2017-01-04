using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Retrieval
{
    public class PolicyIsNotSubmittedRule : IRule<IPolicy>
    {
        public RuleResult IsSatisfiedBy(IPolicy target)
        {
            var notSubmitted = target.Status != PolicyStatus.ReadyForInforce &&
                               target.Status != PolicyStatus.RaisedToPolicyAdminSystem &&
                               target.Status != PolicyStatus.FailedDuringPolicyAdminSystemLoad &&
                               target.Status != PolicyStatus.FailedToSendToPolicyAdminSystem &&
                               target.Status != PolicyStatus.Inforce;

            return new RuleResult(notSubmitted);
        }
    }
}
