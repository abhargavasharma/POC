using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class PersonalDetailsViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        [MaxLength(22, ErrorMessage = "First name cannot be longer than 22 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        [MaxLength(22, ErrorMessage = "Last name cannot be longer than 22 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        public string DateOfBirth { get; set; }

        [MaxLength(50, ErrorMessage = "Address cannot be longer than 50 characters.")]
        [Required(ErrorMessage = "Residential address is required")]
        public string ResidentialAddress { get; set; }

        [MaxLength(80, ErrorMessage = "Email address cannot be longer than 80 characters.")]
        [Required(ErrorMessage = "Email address is required")]
        [IsEmailValidation(ErrorMessage = "A valid Email address must be entered")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [NumbersOnlyValidation(ErrorMessage = "Mobile number must not contain any non numeric values")]
        [IsValidMobilePrefixRuleValidation(ErrorMessage = "Mobile number should start with 04")]
        [StringLengthAllowNullsAndEmpty(MinLength = 10, MaxLength = 10, ErrorMessage = "Mobile number must be 10 digits")]
        public string MobileNumber { get; set; }

        //[Required(ErrorMessage = "Phone number is required")]
        [NumbersOnlyValidation(ErrorMessage = "Home number must not contain any non numeric values")]
        [StartsWithPhoneAreaCodeValidation(ErrorMessage = "Home phone number should start with 02, 03, 07 or 08")]
        [StringLengthAllowNullsAndEmpty(MinLength = 10, MaxLength = 10, ErrorMessage = "Home phone number must be 10 digits")]
        public string HomeNumber { get; set; }

        [MaxLength(20, ErrorMessage = "Suburb cannot be longer than 20 characters.")]
        [Required(ErrorMessage = "Suburb is required")]
        public string Suburb { get; set; }
        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        public string Title { get; set; }

        [Required(ErrorMessage = "Postcode is required")]
        [NumbersOnlyValidation(ErrorMessage = "Postcode must not contain any non numeric values")]
        [StringLengthAllowNullsAndEmpty(MinLength = 4, MaxLength = 4, ErrorMessage = "Postcode must be 4 digits")]
        public string Postcode { get; set; }
    }
}