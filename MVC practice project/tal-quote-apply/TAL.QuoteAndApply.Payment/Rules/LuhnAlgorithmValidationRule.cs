using System;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Payment.Rules
{
    public class LuhnAlgorithmValidationRule : IRule<string>
    {
        private readonly string _validationKey;

        public LuhnAlgorithmValidationRule(string validationKey)
        {
            _validationKey = validationKey;
        }

        public RuleResult IsSatisfiedBy(string target)
        {
            int[] deltas = { 0, 1, 2, 3, 4, -4, -3, -2, -1, 0 };
            var checksum = 0;
            var chars = target.ToCharArray();

            try
            {                
                for (var i = chars.Length - 1; i > -1; i--)
                {
                    var numberAsInt = chars[i] - 48;
                    checksum += numberAsInt;
                    if (((i - chars.Length)%2) == 0)
                        checksum += deltas[numberAsInt];
                }
            }
            catch (IndexOutOfRangeException)
            {
                return new RuleResult(_validationKey, (checksum % 10) == 0, "Not a valid credit card number");
            }

            return new RuleResult(_validationKey, (checksum % 10) == 0, "Not a valid credit card number");
        }
    }
}