using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TAL.QuoteAndApply.ServiceLayer.Payment;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;

namespace TAL.QuoteAndApply.Web.Shared.Attributes.Validation
{
    public class CreditCardNumberValidationAttribute : ValidationAttribute
    {
        private readonly string _cardTypeProperty;

        public CreditCardNumberValidationAttribute(string cardTypeProperty)
        {
            _cardTypeProperty = cardTypeProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult(ErrorMessage, new[] {validationContext.MemberName});
            }

            var field = validationContext.ObjectInstance.GetType().GetProperty(_cardTypeProperty);

            if (field == null)
            {
                throw new InvalidOperationException("CardType property is missing");
            }

            var cardType = (CreditCardType)field.GetValue(validationContext.ObjectInstance, null);

            var paymentOptionService = DependencyResolver.Current.GetService<IPaymentOptionService>();

            var cardNumber = value.ToString();

            return (paymentOptionService.ValidateCardNumber(cardNumber, cardType))
                ? ValidationResult.Success 
                : new ValidationResult(ErrorMessage, new[] { validationContext.MemberName });
        }
    }
}