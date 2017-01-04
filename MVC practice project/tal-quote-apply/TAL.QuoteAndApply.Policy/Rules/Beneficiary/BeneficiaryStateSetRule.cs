using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Beneficiary
{
    public class BeneficiaryStateSetRule : IRule<State>
    {
        private readonly string _key;

        public BeneficiaryStateSetRule(string key)
        {
            _key = key;
        }

        public RuleResult IsSatisfiedBy(State state)
        {
            return RuleResult.ToResult(_key, state != State.Unknown, "State is required");
        }
    }
}