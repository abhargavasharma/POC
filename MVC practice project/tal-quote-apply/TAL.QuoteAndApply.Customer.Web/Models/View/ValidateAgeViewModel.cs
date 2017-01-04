using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.Customer.Web.Models.View
{
    public class ValidateAgeViewModel
    {
        [Required(ErrorMessage = "Date of birth is required")]
        [DateValidation(ErrorMessage = "A valid Date of birth must be entered")]
        public string DateOfBirth { get; set; }
    }
}