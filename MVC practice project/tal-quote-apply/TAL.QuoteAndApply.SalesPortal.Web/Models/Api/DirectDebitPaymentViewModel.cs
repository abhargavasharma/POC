using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class DirectDebitPaymentViewModel
    {
        [Required(ErrorMessage = "Account name is required")]
        [MaxLength(32, ErrorMessage = "Account name cannot be more than 32 characters")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "BSB number is required")]
        [MaxLength(6, ErrorMessage = "BSB number must be exactly 6 Digits")]
        [MinLength(6, ErrorMessage = "BSB number must be exactly 6 Digits")]
        [NumbersOnlyValidation(ErrorMessage = "BSB number must not contain any non numeric values")]
        public string BsbNumber { get; set; }

        [Required(ErrorMessage = "Account number is required")]
        [MaxLength(9, ErrorMessage = "Account number cannot be more than 9 Digits")]
        [MinLength(5, ErrorMessage = "Account number must be at least 5 Digits")]
        [NumbersOnlyValidation(ErrorMessage = "Account number must not contain any non numeric values")]
        public string AccountNumber { get; set; }
        
        public bool IsValidForInforce { get; set; }
    }
}