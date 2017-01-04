using System.Text.RegularExpressions;

namespace TAL.QuoteAndApply.Rules.GenericRules
{
    public abstract class RegexRule : IRule<string>
    {
        protected Regex Regex;

        public RuleResult IsSatisfiedBy(string target)
        {
            var matchResult = Regex.Match(target);
            return new RuleResult(matchResult.Success);
        }
    }

    public class StringContainsAtLeastOneUppercaseLetterRule : RegexRule
    {
        public StringContainsAtLeastOneUppercaseLetterRule()
        {
            Regex = new Regex(@"(?=.*[A-Z])");
        }
    }
    public class StringContainsAtLeastOneLowercaseLettersRule : RegexRule
    {
        public StringContainsAtLeastOneLowercaseLettersRule()
        {
            Regex = new Regex(@"(?=.*[a-z])");
        }
    }


    public class StringContainsAtLeastOneNumberRule : RegexRule
    {
        public StringContainsAtLeastOneNumberRule()
        {
            Regex = new Regex(@"(?=.*[0-9])");
        }
    }

    public class StringContainsAtLeastOneSpecialCharacterRule : RegexRule
    {
        public StringContainsAtLeastOneSpecialCharacterRule()
        {
            Regex = new Regex(@"(?=.*[`~!@#\$\.%\^&\*\(\),\-_\+=\{\}\[\]\|\\';:<>\?/])");
        }
    }
}
