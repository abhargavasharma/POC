using System;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Risk
{
    public class MustBeUnderMaxumumAgeRule : IRule<DateTime>
    {
        private readonly int _maximumAge;
        private readonly string _key;

        public MustBeUnderMaxumumAgeRule(int maximumAge, string key)
        {
            _maximumAge = maximumAge;
            _key = key;
        }

        public RuleResult IsSatisfiedBy(DateTime dateOfBirth)
        {
            var ageNextBirthday = dateOfBirth.Age() + 1;
            var overMaxAge = ageNextBirthday > _maximumAge;
            return new RuleResult(_key, !overMaxAge);
        }
    }

}
