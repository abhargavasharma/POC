using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters
{
    public interface ICreditCardPaymentParamConverter
    {
        CreditCardPaymentParam From(ICreditCardPayment payment, bool isValidForInfoce);
        CreditCardPaymentDto From(CreditCardPaymentParam payment);
    }

    public class CreditCardPaymentParamConverter : ICreditCardPaymentParamConverter
    {
        private readonly ICreditCardTypeConverter _creditCardTypeConverter;

        public CreditCardPaymentParamConverter(ICreditCardTypeConverter creditCardTypeConverter)
        {
            _creditCardTypeConverter = creditCardTypeConverter;
        }

        public CreditCardPaymentParam From(ICreditCardPayment payment, bool isValidForInfoce)
        {
            return new CreditCardPaymentParam
            {
                CardType = _creditCardTypeConverter.From(payment.CardType),
                CardNumber = payment.CardNumber,
                NameOnCard = payment.NameOnCard,
                ExpiryMonth = payment.ExpiryMonth,
                ExpiryYear = payment.ExpiryYear,
                IsValidForInforce = isValidForInfoce,
                Token = payment.Token
            };
        }

        public CreditCardPaymentDto From(CreditCardPaymentParam payment)
        {
            return new CreditCardPaymentDto
            {
                CardNumber = payment.CardNumber,
                NameOnCard = payment.NameOnCard,
                ExpiryMonth = payment.ExpiryMonth,
                ExpiryYear = payment.ExpiryYear
            };
        }
    }
}