using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Beneficiary
{
    public class AtLeastOneBeneficiaryRequiredRule : IRule<IEnumerable<IBeneficiary>>
    {
        private readonly string _key;

        public AtLeastOneBeneficiaryRequiredRule(string key)
        {
            _key = key;
        }

        public RuleResult IsSatisfiedBy(IEnumerable<IBeneficiary> target)
        {
            return new RuleResult(_key, target.Any());
        }
    }
}