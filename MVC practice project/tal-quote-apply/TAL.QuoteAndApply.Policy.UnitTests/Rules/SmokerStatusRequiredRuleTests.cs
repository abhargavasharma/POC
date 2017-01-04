using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules
{
    [TestFixture]
    public class SmokerStatusRequiredRuleTests
    {
        [Test]
        public void IsSatisfiedBy_SmokerStatusNotProvided_IsBroken()
        {
            var smokerStatusRule = new SmokerStatusRequiredRule("");
            var result = smokerStatusRule.IsSatisfiedBy(SmokerStatus.Unknown);

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_SmokerStatusHaventSmoked_IsSatisfied()
        {
            var smokerStatusRule = new SmokerStatusRequiredRule("");
            var result = smokerStatusRule.IsSatisfiedBy(SmokerStatus.No);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_SmokerStatusSmoked10To20_IsSatisfied()
        {
            var smokerStatusRule = new SmokerStatusRequiredRule("");
            var result = smokerStatusRule.IsSatisfiedBy(SmokerStatus.Yes);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_SmokerStatusSmokedMoreThan20_IsSatisfied()
        {
            var smokerStatusRule = new SmokerStatusRequiredRule("");
            var result = smokerStatusRule.IsSatisfiedBy(SmokerStatus.Yes);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_SmokerStatusSmokedLessThan10_IsSatisfied()
        {
            var smokerStatusRule = new SmokerStatusRequiredRule("");
            var result = smokerStatusRule.IsSatisfiedBy(SmokerStatus.No);

            Assert.That(result.IsSatisfied, Is.True);
        }
    }
}