using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Party.Rules
{

    public interface IPartyRuleFactory : IGenericRuleFactory
    {
        IRule<Title> GetTitleIsNotUnkownRule(string key);
        IRule<State> GetStateIsNotUnkownRule(string key);
    }

    public class PartyRuleFactory : GenericRuleFactory, IPartyRuleFactory
    {
        public IRule<State> GetStateIsNotUnkownRule(string key)
        {
            return new StateIsNotUnkownRule(key);
        }

        public IRule<Title> GetTitleIsNotUnkownRule(string key)
        {
            return new TitleIsNotUnkownRule(key);
        }
    }
}
