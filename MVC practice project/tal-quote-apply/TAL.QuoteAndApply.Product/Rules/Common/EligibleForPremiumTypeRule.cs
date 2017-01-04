using System;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Product.Rules.Common
{

    public class EligibleForPremiumTypeRule : IRule<DateTime>
    {
        private readonly PremiumTypeDefinition _premiumTypeDefinition;

        public EligibleForPremiumTypeRule(PremiumTypeDefinition premiumTypeDefinition)
        {
            _premiumTypeDefinition = premiumTypeDefinition;
        }

        public RuleResult IsSatisfiedBy(DateTime dateOfBirth)
        {
            if (_premiumTypeDefinition.MaximumEntryAgeNextBirthday == null)
            {
                return new RuleResult(true);
            }

            var ageNextBirthday = dateOfBirth.Age() + 1;
            var tooOld = ageNextBirthday >= _premiumTypeDefinition.MaximumEntryAgeNextBirthday.Value;
            return new RuleResult(!tooOld);
        }
    }
}
