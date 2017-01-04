using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Party.Rules
{
    public class StateIsNotUnkownRule : IRule<State>
    {
        private readonly string _validationKey;

        public StateIsNotUnkownRule(string validationKey)
        {
            _validationKey = validationKey;
        }

        public RuleResult IsSatisfiedBy(State target)
        {
            return new RuleResult(_validationKey, target != State.Unknown);
        }
    }
}