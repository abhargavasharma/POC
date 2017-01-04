using NUnit.Framework;
using TAL.QuoteAndApply.Rules;
using TAL.QuoteAndApply.ServiceLayer.RulesProxy;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Rules
{
    [TestFixture]
    public class GenericRulesTests
    {

        [TestCase("", Result = false)]
        [TestCase("<6!", Result = false)] //Under 6 chars
        [TestCase(">20 characters, a very long password indeed!", Result = false)]
        [TestCase("No Special Char5", Result = true)]
        [TestCase("No Numbers!", Result = true)]
        [TestCase("none cap!tal l3tters", Result = false)]
        [TestCase("123456789", Result = false)] //Only Numbers
        [TestCase("abcdefghi", Result = false)] //Only Letters
        [TestCase("!@#$%^&*", Result = false)] //Only Special Chars
        [TestCase("ABCDEFGHI", Result = false)] //Only Capitals
        [TestCase("AAbb##12", Result = true)]
        [TestCase("Aabbcc", Result = true)] //One capital letter
        //TODO: More combinations of cases for password complexity
        public bool IsValid_StringIsValidPasswordStrength(string password)
        {
            var genericRules = new GenericRules(new GenericRuleFactory());
            return genericRules.StringIsValidPasswordStrength(password);
        }
    }
}
