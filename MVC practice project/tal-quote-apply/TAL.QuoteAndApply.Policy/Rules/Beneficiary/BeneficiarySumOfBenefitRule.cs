using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Beneficiary
{
    public class BeneficiarySumOfBenefitRule : IRule<IEnumerable<IBeneficiary>>
    {
        private readonly string _key;

        public BeneficiarySumOfBenefitRule(string key)
        {
            _key = key;
        }

        public RuleResult IsSatisfiedBy(IEnumerable<IBeneficiary> target)
        {
            var shareOfBenefit = target.Sum(b => b.Share);
            if (Math.Abs(shareOfBenefit - 100f) > 0)
            {
                return RuleResult.ToResult(_key, false, "Total benefit allocation across all beneficiaries must add up to 100%.");
            }
            return RuleResult.ToResult(true);
        }
    }
}
