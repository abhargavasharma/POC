using System;
using NUnit.Framework;
using TAL.QuoteAndApply.Rules.GenericRules;
namespace TAL.QuoteAndApply.Rules.UnitTests.GenericRules
{
    [TestFixture]
    public class IsValidMobilePrefixRuleTests
    {
        [Test]
        public void IsSatisfied_StringIsNull_ReturnsIsSatisfied()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy(null);

            Assert.That(result.IsSatisfied, Is.True);
        }


        [TestCase("0488888888")]
        public void IsSatisfied_StartsWithValidMobileNumberPrefix_ReturnsIsSatisfied(string mobileNumber)
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy(mobileNumber);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [TestCase("0088888888")]
        [TestCase("0188888888")]
        [TestCase("0588888888")]
        [TestCase("0688888888")]
        [TestCase("0988888888")]
        public void IsSatisfied_StartsWithInvalidMobileNumberPrefix_ReturnsIsBroken(string mobileNumber)
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy(mobileNumber);

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfied_StartsWithCharacter_ReturnsIsBroken()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy("ABC");

            Assert.That(result.IsBroken, Is.True);
        }

        private IRule<string> GetSubject()
        {
            return new IsValidMobilePrefixRule(String.Empty);
        }
    }
}
