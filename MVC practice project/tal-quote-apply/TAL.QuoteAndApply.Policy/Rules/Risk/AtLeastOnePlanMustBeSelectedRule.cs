using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Risk
{
    public class AtLeastOnePlanMustBeSelectedRule : IRule<IEnumerable<IPlan>>
    {
        private readonly string _key;

        public AtLeastOnePlanMustBeSelectedRule(string key)
        {
            _key = key;
        }

        public RuleResult IsSatisfiedBy(IEnumerable<IPlan> target)
        {
            if (target == null)
            {
                return new RuleResult(_key, false);
            }

            return new RuleResult(_key, target.Any(p=> p.Selected));
        }
    }
}