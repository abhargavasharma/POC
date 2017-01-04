using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPaymentOptionsViewModelConverter
    {
        PaymentOptionsViewModel From(PaymentOptionsParam model);
    }

    public class PaymentOptionsViewModelConverter : IPaymentOptionsViewModelConverter
    {
        private readonly ICreditCardPaymentViewModelConverter _creditCardPaymentViewModelConverter;
        private readonly IDirectDebitPaymentViewModelConverter _directDebitPaymentViewModelConverter;
        private readonly ISuperFundPaymentViewModelConverter _superFundPaymentViewModelConverter;

        private readonly ISelfManagedSuperFundPaymentViewModelConverter _selfManagedSuperFundPaymentViewModelConverter;
        public PaymentOptionsViewModelConverter(ICreditCardPaymentViewModelConverter creditCardPaymentViewModelConverter, 
            IDirectDebitPaymentViewModelConverter directDebitPaymentViewModelConverter, 
            ISuperFundPaymentViewModelConverter superFundPaymentViewModelConverter,
            ISelfManagedSuperFundPaymentViewModelConverter selfManagedSuperFundPaymentViewModelConverter)
        {
            _creditCardPaymentViewModelConverter = creditCardPaymentViewModelConverter;
            _directDebitPaymentViewModelConverter = directDebitPaymentViewModelConverter;
            _superFundPaymentViewModelConverter = superFundPaymentViewModelConverter;
            _selfManagedSuperFundPaymentViewModelConverter = selfManagedSuperFundPaymentViewModelConverter;
        }

        public PaymentOptionsViewModel From(PaymentOptionsParam model)
        {
            return new PaymentOptionsViewModel
            {
                CreditCardPayment = _creditCardPaymentViewModelConverter.From(model.CreditCardPayment),
                DirectDebitPayment = _directDebitPaymentViewModelConverter.From(model.DirectDebitPayment),
                SuperannuationPayment = _superFundPaymentViewModelConverter.From(model.SuperannuationPayment),
                SelfManagedSuperFundPayment = _selfManagedSuperFundPaymentViewModelConverter.From(model.SelfManagedSuperFundPayment),
                IsDirectDebitAvailable = model.IsDirectDebitAvailable,
                IsCreditCardAvailable = model.IsCreditCardAvailable,
                IsSuperFundAvailable = model.IsSuperFundAvailable,
                IsSelfManagedSuperFundAvailable = model.IsSelfManagedSuperFundAvailable,
                IsComplete = model.IsComplete
            };
        }
    }
}