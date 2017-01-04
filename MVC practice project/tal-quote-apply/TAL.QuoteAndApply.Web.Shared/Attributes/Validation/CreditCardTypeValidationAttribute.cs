using System.ComponentModel.DataAnnotations;

namespace TAL.QuoteAndApply.Web.Shared.Attributes.Validation
{
    public class CreditCardTypeValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {            
            if (value == null)
            {
                return false;
            }

            var cardType = value;

            return cardType.ToString() == "Visa" || cardType.ToString() == "MasterCard";
        }
    }
}