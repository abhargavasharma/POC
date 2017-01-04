using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Retrieval
{
    public interface IPolicyRetrievalRulesFactory
    {
        IRule<IPolicy> GetMustBeSavedRule();
        IRule<IPolicy> GetNotSubmittedRule();
        IRule<IPolicy> GetNotReferredToUnderwriterRule();
        IRule<IPolicy> GetNotLockedOutRule();
    }

    public class PolicyRetrievalRulesFactory : IPolicyRetrievalRulesFactory
    {
        public IRule<IPolicy> GetMustBeSavedRule()
        {
            return new PolicyIsSavedRule();
        }

        public IRule<IPolicy> GetNotSubmittedRule()
        {
            return new PolicyIsNotSubmittedRule();
        }

        public IRule<IPolicy> GetNotLockedOutRule()
        {
            return new PolicyIsNotLockedOutRule();
        }

        public IRule<IPolicy> GetNotReferredToUnderwriterRule()
        {
            return new PolicyIsNotReferredRule();
        }
    }
}
