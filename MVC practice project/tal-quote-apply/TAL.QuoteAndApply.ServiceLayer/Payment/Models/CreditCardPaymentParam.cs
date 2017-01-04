
namespace TAL.QuoteAndApply.ServiceLayer.Payment.Models
{
    public class CreditCardPaymentParam
    {
        public CreditCardType CardType { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Token { get; set; }

        public bool IsValidForInforce { get; set; }

        public bool Tokenised
        {
            get { return !string.IsNullOrEmpty(Token); }
        }
    }
}