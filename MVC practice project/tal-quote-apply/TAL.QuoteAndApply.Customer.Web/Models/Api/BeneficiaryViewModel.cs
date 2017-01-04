
using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class BeneficiaryViewModel
    {
        [MaxLength(30, ErrorMessage = "Address cannot be longer than 30 characters.")]
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        [MaxLength(20, ErrorMessage = "Suburb cannot be longer than 20 characters.")]
        [Required(ErrorMessage = "Suburb is required")]
        public string Suburb { get; set; }
        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }
        [Required(ErrorMessage = "Postcode is required")]
        [NumbersOnlyValidation(ErrorMessage = "Postcode must not contain any non numeric values")]
        [StringLengthAllowNullsAndEmpty(MinLength = 4, MaxLength = 4, ErrorMessage = "Post code must be 4 digits")]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DateValidation(ErrorMessage = "A valid Date of birth must be entered")]
        public string DateOfBirth { get; set; }
        [Required(ErrorMessage = "Percentage of benefit is required")]
        [NumbersOnlyValidation]
        public string Share { get; set; }
        [Required(ErrorMessage = "Relationship is required")]
        public int? BeneficiaryRelationshipId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [MaxLength(22, ErrorMessage = "First name cannot be longer than 22 characters.")]
        [Required(ErrorMessage = "First name is required")]
        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        public string FirstName { get; set; }
        [MaxLength(22, ErrorMessage = "Last name cannot be longer than 22 characters.")]
        [Required(ErrorMessage = "Last name is required")]
        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        public string Surname { get; set; }

        [NumbersOnlyValidation(ErrorMessage = "Phone number must not contain any non numeric values")]
        [IsValidMobileOrLandlinePrefixRuleValidation(ErrorMessage = "Phone number should start with 02, 03, 04, 07 or 08")]
        [StringLengthAllowNullsAndEmpty(MinLength = 10, MaxLength = 10, ErrorMessage = "Phone number must be 10 digits")]
        public string PhoneNumber { get; set; }

        [MaxLength(80, ErrorMessage = "Email address cannot be longer than 80 characters.")]
        [IsEmailValidation(ErrorMessage = "A valid Email address must be entered")]
        public string EmailAddress { get; set; }

        public int RiskId { get; set; }
        public int Id { get; set; }
        
        public bool IsCompleted { get; set; }
        
        public bool HasEmptyFields()
        {
            return Address == null ||
                   DateOfBirth == null ||
                   FirstName == null ||
                   BeneficiaryRelationshipId == null ||
                   Surname == null;
        }
    }
}