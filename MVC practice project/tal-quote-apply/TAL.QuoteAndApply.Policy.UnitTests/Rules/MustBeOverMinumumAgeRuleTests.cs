using System;
using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules
{
    [TestFixture]
    public class MustBeOverMinumumAgeRuleTests
    {
        private MustBeOverMinumumAgeRule _mustBeOverMinAgeRule;

        [SetUp]
        public void Setup()
        {
            var defaultPlanDefinition = new RiskProductDefinition { MinimumEntryAgeNextBirthday = 19, MaximumEntryAgeNextBirthday = 75 };
            _mustBeOverMinAgeRule = new MustBeOverMinumumAgeRule(defaultPlanDefinition.MinimumEntryAgeNextBirthday, "");
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(17)]
        public void IsSatisfiedBy_AgeUnderMinimum_IsBroken(int age)
        {
            var dateOfBirth = DateTime.Now.Date.AddYears(-age);
            var result = _mustBeOverMinAgeRule.IsSatisfiedBy(dateOfBirth);

            Assert.That(result.IsBroken, Is.True);
        }

        [TestCase(18)]
        [TestCase(19)]
        [TestCase(100)]
        public void IsSatisfiedBy_AgeEqualToOrOverMinimum_IsSatisfied(int age)
        {
            var dateOfBirth = DateTime.Now.Date.AddYears(-age);
            var result = _mustBeOverMinAgeRule.IsSatisfiedBy(dateOfBirth);

            Assert.That(result.IsSatisfied, Is.True);
        }
    }
}