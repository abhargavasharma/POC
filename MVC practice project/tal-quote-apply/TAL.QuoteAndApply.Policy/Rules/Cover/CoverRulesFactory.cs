using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Cover
{
    public interface ICoverRulesFactory
    {
        IRule<ICover> GetSelectedCoverNotUnderwritingDeclinedRule();
        IRule<ICover> GetCoverNotUnderwritingDeclinedRule();
    }
    public class CoverRulesFactory : ICoverRulesFactory
    {
        public IRule<ICover> GetSelectedCoverNotUnderwritingDeclinedRule()
        {
            return new SelectedCoverNotUnderwritingDeclinedRule();
        }

        public IRule<ICover> GetCoverNotUnderwritingDeclinedRule()
        {
            return new CoverNotUnderwritingDeclinedRule();
        }
    }
}
