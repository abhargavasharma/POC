using System;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Beneficiary
{
    public class BeneficiaryDateOfBirthSetRule : IRule<DateTime?>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly string _key;

        public BeneficiaryDateOfBirthSetRule(IDateTimeProvider dateTimeProvider, string key)
        {
            _dateTimeProvider = dateTimeProvider;
            _key = key;
        }

        public RuleResult IsSatisfiedBy(DateTime? target)
        {
            return RuleResult.ToResult(_key,
                target.HasValue && target.Value != DateTime.MinValue &&
                target.Value < _dateTimeProvider.GetCurrentDate(), "Date of birth is required");
        }
    }
}