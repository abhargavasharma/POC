using NUnit.Framework;
using TAL.QuoteAndApply.Rules.GenericRules;

namespace TAL.QuoteAndApply.Rules.UnitTests.GenericRules
{
    [TestFixture]
    public class StringIsNumbersOnlyRuleTests
    {
        [Test]
        public void IsSatisfied_StringIsNull_ReturnsIsSatisfied()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy(null);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfied_StringIsEmpty_ReturnsIsSatisfied()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy("");

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfied_StringHasNumbersOnly_ReturnsIsSatisfied()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy("12345");

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfied_StringHasNumbersAndLetters_ReturnsIsBroken()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy("1a1");

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfied_StringHasNumbersAndCharacters_ReturnsIsBroken()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy("1!");

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfied_StringHasLettersOnly_ReturnsIsBroken()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy("a");

            Assert.That(result.IsBroken, Is.True);
        }

        private IRule<string> GetSubject()
        {
            return new StringIsNumbersOnlyRule(string.Empty);
        }
    }
}