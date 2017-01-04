using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;

namespace TAL.QuoteAndApply.Customer.Web.Services.Converters
{
    public interface IPaymentOptionsViewModelConverter
    {
        PaymentOptionsViewModel From(PaymentOptionsParam model);
    }

    public class PaymentOptionsViewModelConverter : IPaymentOptionsViewModelConverter
    {
        private readonly ICreditCardPaymentViewModelConverter _creditCardPaymentViewModelConverter;
        private readonly IDirectDebitPaymentViewModelConverter _directDebitPaymentViewModelConverter;

        public PaymentOptionsViewModelConverter(ICreditCardPaymentViewModelConverter creditCardPaymentViewModelConverter, 
            IDirectDebitPaymentViewModelConverter directDebitPaymentViewModelConverter)
        {
            _creditCardPaymentViewModelConverter = creditCardPaymentViewModelConverter;
            _directDebitPaymentViewModelConverter = directDebitPaymentViewModelConverter;
        }

        public PaymentOptionsViewModel From(PaymentOptionsParam model)
        {
            return new PaymentOptionsViewModel
            {
                CreditCardPayment = _creditCardPaymentViewModelConverter.From(model.CreditCardPayment),        
                IsCreditCardSelected = model.IsCreditCardAvailable,
                DirectDebitPayment = _directDebitPaymentViewModelConverter.From(model.DirectDebitPayment),
                IsDirectDebitSelected = model.IsDirectDebitAvailable
            };
        }
    }
}