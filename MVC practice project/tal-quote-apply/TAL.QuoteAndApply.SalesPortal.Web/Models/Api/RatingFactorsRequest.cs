using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class RatingFactorsRequest
    {
        [Required(ErrorMessage = "Gender is required")]
        public char? Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DateValidation(ErrorMessage = "A valid Date of birth must be entered")]
        public string DateOfBirth { get; set; }

        [Required(ErrorMessage = "Residency status is required")]
        public bool? AustralianResident { get; set; }

        public string SmokerStatus { get; set; }

        [Required(ErrorMessage = "Occupation is required")]
        public string OccupationCode { get; set; }
        public string OccupationTitle { get; set; }

        public string IndustryCode { get; set; }
        public string IndustryTitle { get; set; }

        [Required(ErrorMessage = "Annual income is required")]
        [Range(0, long.MaxValue, ErrorMessage = "Annual income is required")]
        public long? Income { get; set; }
    }
}