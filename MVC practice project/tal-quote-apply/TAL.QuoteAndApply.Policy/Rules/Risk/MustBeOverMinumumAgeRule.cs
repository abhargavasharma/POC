using System;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Risk
{
    public class MustBeOverMinumumAgeRule : IRule<DateTime>
    {
        private readonly int _minAge;
        private readonly string _key;

        public MustBeOverMinumumAgeRule(int minAge, string key)
        {
            _minAge = minAge;
            _key = key;
        }

        public RuleResult IsSatisfiedBy(DateTime dateOfBirth)
        {
            var ageNextBirthday = dateOfBirth.Age() + 1;
            var underAge = ageNextBirthday < _minAge;
            return new RuleResult(_key, !underAge);
        }
    }

}
