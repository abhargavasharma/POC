using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models
{
    public class RaisePolicyCreditCard : ICreditCardPayment
    {
        public CreditCardType CardType { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Token { get; set; }
        public int Id { get; set; }
        public int PolicyPaymentId { get; set; }
    }
}