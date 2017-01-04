using System.Monads;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface ICreditCardPaymentViewModelConverter
    {
        CreditCardPaymentViewModel From(CreditCardPaymentParam creditCardPaymentParam);
        CreditCardPaymentParam From(CreditCardPaymentViewModel creditCardPaymentParam);
    }

    public class CreditCardPaymentViewModelConverter : ICreditCardPaymentViewModelConverter
    {
        private readonly ICreditCardTypeConverter _creditCardTypeConverter;

        public CreditCardPaymentViewModelConverter(ICreditCardTypeConverter creditCardTypeConverter)
        {
            _creditCardTypeConverter = creditCardTypeConverter;
        }

        public CreditCardPaymentViewModel From(CreditCardPaymentParam creditCardPaymentParam)
        {
            return creditCardPaymentParam.IsNotNull()
                ? new CreditCardPaymentViewModel
                {
                    CardType = creditCardPaymentParam.CardType,
                    CardNumber = creditCardPaymentParam.CardNumber,
                    ExpiryMonth = creditCardPaymentParam.ExpiryMonth,
                    ExpiryYear = creditCardPaymentParam.ExpiryYear,
                    NameOnCard = creditCardPaymentParam.NameOnCard,
                    Token = creditCardPaymentParam.Token,
                    IsValidForInforce = creditCardPaymentParam.IsValidForInforce
                }
                : new CreditCardPaymentViewModel();
        }

        public CreditCardPaymentParam From(CreditCardPaymentViewModel creditCardPaymentParam)
        {
            return new CreditCardPaymentParam
            {
                CardType = creditCardPaymentParam.CardType,
                CardNumber = creditCardPaymentParam.CardNumber,
                ExpiryMonth = creditCardPaymentParam.ExpiryMonth,
                ExpiryYear = creditCardPaymentParam.ExpiryYear,
                NameOnCard = creditCardPaymentParam.NameOnCard,
                Token = creditCardPaymentParam.Token,
                IsValidForInforce = creditCardPaymentParam.IsValidForInforce
            };
        }
    }
}