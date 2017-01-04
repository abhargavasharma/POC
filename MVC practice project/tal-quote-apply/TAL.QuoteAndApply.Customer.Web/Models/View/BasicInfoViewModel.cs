using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.Customer.Web.Models.View
{
    public class BasicInfoViewModel
    {
        [Required(ErrorMessage = "Date of birth is required")]
        [DateValidation(ErrorMessage = "A valid Date of birth must be entered")]
        public string DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public char? Gender { get; set; }
        [Required(ErrorMessage = "Smoker status is required")]
        public bool? IsSmoker { get; set; }
        [Required(ErrorMessage = "Annual income is required")]
        public long? AnnualIncome { get; set; }
        public PolicySource Source { get; set; }

        [Required(ErrorMessage = "A valid Occupation is required")]
        public string OccupationCode { get; set; }
        public string IndustryCode { get; set; }
        public string RecaptchaResponse { get; set; }

        [Required(ErrorMessage = "Postcode is required")]
        [PostcodeValidation(ErrorMessage = "Postcode is invalid")]
        public string Postcode { get; set; }
    }
}