using System.Monads;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;

namespace TAL.QuoteAndApply.Customer.Web.Services.Converters
{
    public interface ICreditCardPaymentViewModelConverter
    {
        CreditCardPaymentViewModel From(CreditCardPaymentParam creditCardPaymentParam);
        CreditCardPaymentParam From(CreditCardPaymentViewModel creditCardPaymentParam);
    }

    public class CreditCardPaymentViewModelConverter : ICreditCardPaymentViewModelConverter
    {
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