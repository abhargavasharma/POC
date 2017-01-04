
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class PersonalDetailsRequest
    {
        public string Title { get; set; }
        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        [MaxLength(22, ErrorMessage = "First name cannot be longer than 22 characters.")]
        public string FirstName { get; set; }
        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        [MaxLength(22, ErrorMessage = "Surname cannot be longer than 22 characters.")]
        public string Surname { get; set; }

        [MaxLength(50, ErrorMessage = "Address cannot be longer than 50 characters.")]
        public string Address { get; set; }
        [MaxLength(20, ErrorMessage = "Suburb cannot be longer than 20 characters.")]
        public string Suburb { get; set; }
        public string State { get; set; }

        [NumbersOnlyValidation(ErrorMessage = "Postcode must not contain any non numeric values")]
        [StringLengthAllowNullsAndEmpty(MinLength = 4, MaxLength = 4, ErrorMessage = "Postcode must be 4 digits")]
        public string Postcode { get; set; }

        [NumbersOnlyValidation(ErrorMessage = "Mobile number must not contain any non numeric values")]
        [IsValidMobilePrefixRuleValidation(ErrorMessage = "Mobile number should start with 04")]
        [StringLengthAllowNullsAndEmpty(MinLength = 10, MaxLength = 10, ErrorMessage = "Mobile number must be 10 digits")]        
        public string MobileNumber { get; set; }

        [NumbersOnlyValidation(ErrorMessage = "Home number must not contain any non numeric values")]
        [StartsWithPhoneAreaCodeValidation(ErrorMessage = "Home phone number should start with 02, 03, 07 or 08")]
        [StringLengthAllowNullsAndEmpty(MinLength = 10, MaxLength = 10, ErrorMessage = "Home phone number must be 10 digits")]
        public string HomeNumber { get; set; }        

        [IsEmailValidation(ErrorMessage = "A valid Email address must be entered")]
        [MaxLength(80, ErrorMessage = "Email address cannot be longer than 80 characters.")]
        public string EmailAddress { get; set; }

        [MaxLength(20, ErrorMessage = "External customer reference cannot be longer than 20 characters.")]
        public string ExternalCustomerReference { get; set; }

        public bool IsCompleted { get; set; }
        public List<string> PartyConsents { get; set; }
        public bool ExpressConsent { get; set; }
        public long? LeadId { get; set; }
    }
}