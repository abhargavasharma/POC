using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class SuperFundPaymentViewModel
    {
        [Required(ErrorMessage = "Fund name is required")]
        public string FundName { get; set; }

        [Required(ErrorMessage = "Fund USI is required")]
        [MaxLength(14, ErrorMessage = "Fund USI should not exceed 14 Digits")]
        public string FundUSI { get; set; }
        
        [Required(ErrorMessage = "Fund ABN is required")]
        [NumbersOnlyValidation(ErrorMessage = "Fund ABN must not contain any non numeric values")]
        [MaxLength(11, ErrorMessage = "ABN should exactly 11 Digits")]
        [MinLength(11, ErrorMessage = "ABN should exactly 11 Digits")]
        public string FundABN { get; set; }

        [Required(ErrorMessage = "Fund product is required")]
        public string FundProduct { get; set; }

        [Required(ErrorMessage = "Fund membership number is required")]
        public string MembershipNumber { get; set; }

        [Required(ErrorMessage = "Tax file number is required")]
        [NumbersOnlyValidation(ErrorMessage = "Tax file number must not contain any non numeric values")]
        [MaxLength(9, ErrorMessage = "Tax file number should be exactly 9 Digits")]
        [MinLength(9, ErrorMessage = "Tax file number should be exactly 9 Digits")]
        public string TaxFileNumber { get; set; }
        
        public bool IsValidForInforce { get; set; }
    }
}