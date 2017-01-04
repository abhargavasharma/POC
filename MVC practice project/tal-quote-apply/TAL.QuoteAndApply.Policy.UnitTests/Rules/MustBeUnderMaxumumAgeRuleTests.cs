using System;
using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules
{
    [TestFixture]
    public class MustBeUnderMaxumumAgeRuleTests
    {
        private MustBeUnderMaxumumAgeRule _mustBeUnderMaxAgeRule;

        [SetUp]
        public void Setup()
        {
            var defaultPlanDefinition = new RiskProductDefinition { MinimumEntryAgeNextBirthday = 19, MaximumEntryAgeNextBirthday = 75 };
            _mustBeUnderMaxAgeRule = new MustBeUnderMaxumumAgeRule(defaultPlanDefinition.MaximumEntryAgeNextBirthday, "");
        }

        [TestCase(75)]
        [TestCase(76)]
        public void IsSatisfiedBy_AgeEqualOrOverMaximum_IsBroken(int age)
        {
            var dateOfBirth = DateTime.Now.Date.AddYears(-age);
            var result = _mustBeUnderMaxAgeRule.IsSatisfiedBy(dateOfBirth);

            Assert.That(result.IsBroken, Is.True);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(73)]
        [TestCase(74)]
        public void IsSatisfiedBy_AgeUnderMaximum_IsSatisfied(int age)
        {
            var dateOfBirth = DateTime.Now.Date.AddYears(-age);
            var result = _mustBeUnderMaxAgeRule.IsSatisfiedBy(dateOfBirth);

            Assert.That(result.IsSatisfied, Is.True);
        }
    }
}