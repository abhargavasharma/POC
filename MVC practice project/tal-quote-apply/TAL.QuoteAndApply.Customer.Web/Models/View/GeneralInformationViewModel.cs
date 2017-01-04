using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.Customer.Web.Models.View
{
    public class GeneralInformationViewModel : ValidateAgeViewModel
    {
        [Required(ErrorMessage = "Postcode is required")]
        [PostcodeValidation(ErrorMessage = "Postcode is invalid")]
        public string Postcode { get; set; }

        public string RecaptchaResponse { get; set; }
    }
}