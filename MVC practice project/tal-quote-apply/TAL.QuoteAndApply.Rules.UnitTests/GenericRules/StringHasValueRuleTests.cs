using NUnit.Framework;
using TAL.QuoteAndApply.Rules.GenericRules;

namespace TAL.QuoteAndApply.Rules.UnitTests.GenericRules
{
    [TestFixture]
    public class StringHasValueRuleTests
    {
        [Test]
        public void IsSatisfied_StringIsNull_ReturnsIsBroken()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy(null);

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfied_StringIsEmpty_ReturnsIsBroken()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy("");

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfied_StringIsWhiteSpace_ReturnsIsBroken()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy(" ");

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfied_StringHasValues_ReturnsIsSatisfied()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy("asdsad");

            Assert.That(result.IsSatisfied, Is.True);
        }

        private IRule<string> GetSubject()
        {
            return new StringHasValueRule(string.Empty);
        }
    }
}