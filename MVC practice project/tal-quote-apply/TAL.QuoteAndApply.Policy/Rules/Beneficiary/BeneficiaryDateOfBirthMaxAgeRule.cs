using System;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Beneficiary
{
    //This is to get around the sql exception where dateTime is outside the limits of the sql storage limit for dateTime
    public class BeneficiaryDateOfBirthMaxAgeRule : IRule<DateTime?>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly string _key;

        public BeneficiaryDateOfBirthMaxAgeRule(IDateTimeProvider dateTimeProvider, string key)
        {
            _dateTimeProvider = dateTimeProvider;
            _key = key;
        }

        public RuleResult IsSatisfiedBy(DateTime? target)
        {
            return RuleResult.ToResult(_key,
                target.HasValue &&
                target.Value > _dateTimeProvider.GetCurrentDate().AddYears(-200), "A valid Date of birth must be entered");
        }
    }
}