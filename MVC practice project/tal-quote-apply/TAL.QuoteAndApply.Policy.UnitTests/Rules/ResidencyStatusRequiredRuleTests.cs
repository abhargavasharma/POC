using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules
{
    [TestFixture]
    public class ResidencyStatusRequiredRuleTests
    {
        [Test]
        public void IsSatisfiedBy_ResidencyStatusNotProvided_IsBroken()
        {
            var residencyStatusRequiredRule = new ResidencyStatusRequiredRule("");
            var result = residencyStatusRequiredRule.IsSatisfiedBy(ResidencyStatus.Unknown);

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_ResidencyStatusNonAustralian_IsSatisfied()
        {
            var residencyStatusRequiredRule = new ResidencyStatusRequiredRule("");
            var result = residencyStatusRequiredRule.IsSatisfiedBy(ResidencyStatus.NonAustralian);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_ResidencyStatusAustralian_IsSatisfied()
        {
            var residencyStatusRequiredRule = new ResidencyStatusRequiredRule("");
            var result = residencyStatusRequiredRule.IsSatisfiedBy(ResidencyStatus.Australian);

            Assert.That(result.IsSatisfied, Is.True);
        }
    }
}