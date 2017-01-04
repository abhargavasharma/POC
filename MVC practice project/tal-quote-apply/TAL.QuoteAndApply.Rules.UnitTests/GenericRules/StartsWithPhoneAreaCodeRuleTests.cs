using NUnit.Framework;
using System;
using TAL.QuoteAndApply.Rules.GenericRules;

namespace TAL.QuoteAndApply.Rules.UnitTests.GenericRules
{
    [TestFixture]
    public class StartsWithPhoneAreaCodeRuleTests
    {
        [Test]
        public void IsSatisfied_StringIsNull_ReturnsIsSatisfied()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy(null);

            Assert.That(result.IsSatisfied, Is.True);
        }
        
        [TestCase("0288888888")]
        [TestCase("0388888888")]
        [TestCase("0788888888")]
        [TestCase("0888888888")]
        public void IsSatisfied_StartsWithValidAreaCode_ReturnsIsSatisfied(string phoneNumber)
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy(phoneNumber);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [TestCase("0088888888")]
        [TestCase("0188888888")]
        [TestCase("0588888888")]
        [TestCase("0688888888")]
        [TestCase("0988888888")]
        public void IsSatisfied_StartsWithInvalidAreaCode_ReturnsIsBroken(string phoneNumber)
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy(phoneNumber);

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
            return new StartsWithPhoneAreaCodeRule(String.Empty);
        }
    }
}