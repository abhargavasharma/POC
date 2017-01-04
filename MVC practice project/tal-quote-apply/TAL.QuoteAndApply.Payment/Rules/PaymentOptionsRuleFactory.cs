using System.Text.RegularExpressions;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Payment.Rules
{
    public interface IPaymentOptionsRuleFactory : IGenericRuleFactory
    {
        IRule<string> GetLuhnAlgorithmValidationRule(string validationKey);
    }

    public class PaymentOptionsRuleFactory : GenericRuleFactory, IPaymentOptionsRuleFactory
    {
        public IRule<string> GetLuhnAlgorithmValidationRule(string validationKey)
        {
            return new LuhnAlgorithmValidationRule(validationKey);
        }        
    }
}
