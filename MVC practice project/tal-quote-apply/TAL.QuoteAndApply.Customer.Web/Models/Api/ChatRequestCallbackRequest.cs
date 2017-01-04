using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class ChatRequestCallbackRequest
    {
        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        [Required(ErrorMessage = "First name is required")]
        [MaxLength(22, ErrorMessage = "First name cannot be longer than 22 characters.")]
        public string FirstName { get; set; }

        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(22, ErrorMessage = "Last name cannot be longer than 22 characters.")]
        public string LastName { get; set; }

        [NumbersOnlyValidation(ErrorMessage = "Phone number must not contain any non numeric values")]
        [IsValidMobileOrLandlinePrefixRuleValidation(ErrorMessage = "Phone number should start with 02, 03, 04, 07 or 08")]
        [StringLengthAllowNullsAndEmpty(MinLength = 10, MaxLength = 10, ErrorMessage = "Phone number must be 10 digits")]
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }

        [IsEmailValidation(ErrorMessage = "A valid Email address must be entered")]
        [MaxLength(80, ErrorMessage = "Email address cannot be longer than 80 characters.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Risk Id is required")]
        public int? RiskId { get; set; }
    }
}