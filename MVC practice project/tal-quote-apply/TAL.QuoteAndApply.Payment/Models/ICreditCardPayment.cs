using TAL.QuoteAndApply.DataModel.Payment;

namespace TAL.QuoteAndApply.Payment.Models
{
    public interface ICreditCardPayment : IPayment
    {
        CreditCardType CardType { get; set; }
        string NameOnCard { get; set; }
        string CardNumber { get; set; }
        string ExpiryMonth { get; set; }
        string ExpiryYear { get; set; }
        string Token { get; set; }
        int Id { get; set; }
    }
}