using NUnit.Framework;
using TAL.QuoteAndApply.Rules.GenericRules;

namespace TAL.QuoteAndApply.Rules.UnitTests.GenericRules
{
    [TestFixture]
    public class StringIsValidPasswordStrengthRuleTests
    {
        [TestCase("", Result = false)]
        [TestCase("a", Result = false)]
        [TestCase("aa", Result = false)]
        [TestCase("AA", Result = true)]
        [TestCase("ABCDEF", Result = true)]
        [TestCase("kjhkjh&&A", Result = true)]
        public bool IsSatisfied_StringContainsAtLeastOneUppercaseLetterRule(string password)
        {
            var rule = new StringContainsAtLeastOneUppercaseLetterRule();
            var ruleResult = rule.IsSatisfiedBy(password);
            return ruleResult.IsSatisfied;
        }

        [TestCase("", Result = false)]
        [TestCase("A", Result = false)]
        [TestCase("AA", Result = false)]
        [TestCase("aa", Result = true)]
        [TestCase("abcdef", Result = true)]
        [TestCase("aTHKJHKJHg", Result = true)]
        public bool IsSatisfied_StringContainsAtLeastOneLowercaseLettersRule(string password)
        {
            var rule = new StringContainsAtLeastOneLowercaseLettersRule();
            var ruleResult = rule.IsSatisfiedBy(password);
            return ruleResult.IsSatisfied;
        }

        [TestCase("", Result = false)]
        [TestCase("a", Result = false)]
        [TestCase("1234", Result = false)]
        [TestCase("asfsdf243", Result = false)]
        [TestCase("asdsd@fs", Result = true)]
        [TestCase("!", Result = true)]
        [TestCase("@", Result = true)]
        [TestCase("#", Result = true)]
        [TestCase("$", Result = true)]
        [TestCase("&", Result = true)]
        [TestCase("*", Result = true)]
        [TestCase("~", Result = true)]
        [TestCase("`", Result = true)]
        [TestCase("%", Result = true)]
        [TestCase("^", Result = true)]
        [TestCase("(", Result = true)]
        [TestCase(")", Result = true)]
        [TestCase("_", Result = true)]
        [TestCase("-", Result = true)]
        [TestCase("+", Result = true)]
        [TestCase("=", Result = true)]
        [TestCase("{", Result = true)]
        [TestCase("}", Result = true)]
        [TestCase("[", Result = true)]
        [TestCase("]", Result = true)]
        [TestCase("|", Result = true)]
        [TestCase("\\", Result = true)]
        [TestCase("/", Result = true)]
        [TestCase(",", Result = true)]
        [TestCase(".", Result = true)]
        [TestCase("?", Result = true)]
        [TestCase("<", Result = true)]
        [TestCase(">", Result = true)]
        [TestCase("!@#$&*", Result = true)]
        public bool IsSatisfied_StringContainsAtLeastOneSpecialCharacterRule(string password)
        {
            var rule = new StringContainsAtLeastOneSpecialCharacterRule();
            var ruleResult = rule.IsSatisfiedBy(password);
            return ruleResult.IsSatisfied;
        }

        [TestCase("", Result = false)]
        [TestCase("!@#$&*", Result = false)]
        [TestCase("1", Result = true)]
        [TestCase("sdf4dsf", Result = true)]
        [TestCase("123456", Result = true)]
        public bool IsSatisfied_StringContainsAtLeastOneNumberRule(string password)
        {
            var rule = new StringContainsAtLeastOneNumberRule();
            var ruleResult = rule.IsSatisfiedBy(password);
            return ruleResult.IsSatisfied;
        }
    }
}
