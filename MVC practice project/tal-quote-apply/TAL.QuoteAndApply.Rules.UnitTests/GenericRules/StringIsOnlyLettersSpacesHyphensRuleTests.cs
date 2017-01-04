using NUnit.Framework;
using TAL.QuoteAndApply.Rules.GenericRules;

namespace TAL.QuoteAndApply.Rules.UnitTests.GenericRules
{
    [TestFixture]
    public class StringIsOnlyLettersSpacesHyphensRuleTests
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
        public void IsSatisfied_StringIsLettersOnly_ReturnsIsSatisfied()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy("AaBb");

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfied_StringIsHyphenOnly_ReturnsIsSatisfied()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy("-");

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfied_StringIsSpaceOnly_ReturnsIsSatisfied()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy(" ");

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfied_StringIsNumbers_ReturnsIsBroken()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy("1234");

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfied_StringIsCharacters_ReturnsIsBroken()
        {
            var rule = GetSubject();
            var result = rule.IsSatisfiedBy("%^%^");

            Assert.That(result.IsBroken, Is.True);
        }

        private IRule<string> GetSubject()
        {
            return new StringIsOnlyLettersSpacesHyphensRule(string.Empty);
        }
    }
}
