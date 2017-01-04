using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Payment;

namespace TAL.QuoteAndApply.Payment.Models
{
    public class CreditCardPaymentDto : DbItem, ICreditCard, ICreditCardPayment
    {
        public CreditCardType CardType { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Token { get; set; }
        public int PolicyPaymentId { get; set; }
    }
}
