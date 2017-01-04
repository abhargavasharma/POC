using System;
using NUnit.Framework;
using TAL.QuoteAndApply.Rules.GenericRules;

namespace TAL.QuoteAndApply.Rules.UnitTests.GenericRules
{
    [TestFixture]
    public class StringIsBetweenMinAndMaxLengthTests
    {
        [Test]
        public void IsSatisfied_StringIsNull_ReturnsIsSatisfied()
        {
            var rule = GetSubject(0,0);
            var result = rule.IsSatisfiedBy(null);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfied_StringIsEmpty_ReturnsIsSatisfied()
        {
            var rule = GetSubject(0, 0);
            var result = rule.IsSatisfiedBy("");

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfied_StringIsLessThanMinLength_ReturnsIsBroken()
        {
            var rule = GetSubject(2, 10);
            var result = rule.IsSatisfiedBy("1");

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfied_StringIsGreaterThanMaxLength_ReturnsIsBroken()
        {
            var rule = GetSubject(1, 2);
            var result = rule.IsSatisfiedBy("123");

            Assert.That(result.IsBroken, Is.True);
        }

        [TestCase(1,3, "a")]
        [TestCase(1, 3, "ab")]
        [TestCase(1, 3, "abc")]
        [TestCase(3, 3, "abc")]
        public void IsSatisfied_ValidCases_ReturnsIsSatsified(int minLength, int maxLength, string val)
        {
            var rule = GetSubject(minLength, maxLength);
            var result = rule.IsSatisfiedBy(val);

            Assert.That(result.IsSatisfied, Is.True);
        }

        private IRule<string> GetSubject(int minLength, int maxLength)
        {
            return new StringIsBetweenMinAndMaxLengthRule(String.Empty, minLength, maxLength);
        }
    }
}