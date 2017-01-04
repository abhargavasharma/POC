using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class BeneficiaryOptionsRequest
    {
        public bool NominateLpr { get; set; }
    }

    public class BeneficiaryDetailsRequest
    {
        [MaxLength(30, ErrorMessage = "Address cannot be longer than 30 characters.")]
        public string Address { get; set; }

        [MaxLength(20, ErrorMessage = "Suburb cannot be longer than 20 characters.")]
        public string Suburb { get; set; }

        public string State { get; set; }

        [NumbersOnlyValidation(ErrorMessage = "Postcode must not contain any non numeric values")]
        [StringLengthAllowNullsAndEmpty(MinLength = 4, MaxLength = 4, ErrorMessage = "Post code must be 4 digits")]
        public string Postcode { get; set; }

        public string DateOfBirth { get; set; }

        [NumbersOnlyValidation]
        public string Share { get; set; }

        public int? BeneficiaryRelationshipId { get; set; }


        public string Title { get; set; }
        [MaxLength(22, ErrorMessage = "First name cannot be longer than 22 characters.")]
        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        public string FirstName { get; set; }
        [MaxLength(22, ErrorMessage = "Surname cannot be longer than 22 characters.")]
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
                   //Email address removed as no longer mandatory
                   //EmailAddress == null ||
                   FirstName == null ||
                   PhoneNumber == null ||
                   Postcode == null ||
                   BeneficiaryRelationshipId == null ||
                   State == null ||
                   Title == null ||
                   Surname == null ||
                   Suburb == null;
        }
    }
}