using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class CreditCardPaymentViewModel
    {
        [CreditCardTypeValidation(ErrorMessage = "Card type is required")]
        public CreditCardType CardType { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string NameOnCard { get; set; }

        [RequiredIf("Token", RequiredIfCheckType.Null, RequiredIfCheckType.StringEmpty, ErrorMessage = "Card number is required")]
        [MaxLength(16, ErrorMessage = "Card number cannot be more than 16 digits")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Card number invalid: Only digits accepted")]
        [CreditCardNumberValidation("CardType", ErrorMessage = "Card type does not match Card number")]
        public string CardNumber { get; set; }

        [RequiredIf("Token", RequiredIfCheckType.Null, ErrorMessage = "Expiry month is required")]
        public string ExpiryMonth { get; set; }

        [RequiredIf("Token", RequiredIfCheckType.Null, ErrorMessage = "Expiry year is required")]
        public string ExpiryYear { get; set; }

        public string Token { get; set; }
        public bool IsValidForInforce { get; set; }
    }
}