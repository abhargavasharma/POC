using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    //todo: fix required if attribute

    public class SearchClientsRequest
    {
        public bool SearchOnQuoteReference { get; set; }

        [RequiredIf("SearchOnQuoteReference", true, ErrorMessage = "Quote Reference is required")]
        [MinLength(10, ErrorMessage = "Quote reference must be 10 characters long")]
        [MaxLength(10, ErrorMessage = "Quote reference must be 10 characters long")]
        public string QuoteReferenceNumber { get; set; }

        public bool SearchOnLeadId { get; set; }
        
        [RequiredIf("SearchOnLeadId", true, ErrorMessage = "Adobe Id is required")]
        [NumbersOnlyValidation(ErrorMessage = "Adobe Id must be a number")]
        public string LeadId { get; set; }

        public bool SearchOnParty { get; set; }

        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        [RequiredIf("NoPartyFieldsEntered", true, ErrorMessage = "At least one field is required")]
        public string FirstName { get; set; }

        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        [RequiredIf("NoPartyFieldsEntered", true, ErrorMessage = "At least one field is required")]
        public string Surname { get; set; }

        [RequiredIf("NoPartyFieldsEntered", true, ErrorMessage = "At least one field is required")]
        [DateValidation(ErrorMessage = "A valid Date of birth must be entered")]
        public string DateOfBirth { get; set; }

        [RequiredIf("NoPartyFieldsEntered", true, ErrorMessage = "At least one field is required")]
        [IsValidMobilePrefixRuleValidation(ErrorMessage = "Mobile number should start with 04")]
        [NumbersOnlyValidation(ErrorMessage = "Mobile number must not contain any non numeric values")]
        [StringLengthAllowNullsAndEmpty(MinLength = 10, MaxLength = 10, ErrorMessage = "Mobile number must be 10 digits")]
        public string MobileNumber { get; set; }

        [RequiredIf("NoPartyFieldsEntered", true, ErrorMessage = "At least one field is required")]
        [NumbersOnlyValidation(ErrorMessage = "Home number must not contain any non numeric values")]
        [StartsWithPhoneAreaCodeValidation(ErrorMessage = "Home number should start with 02, 03, 07 or 08")]
        [StringLengthAllowNullsAndEmpty(MinLength = 10, MaxLength = 10, ErrorMessage = "Home number must be 10 digits")]
        public string HomeNumber { get; set; }

        [IsEmailValidation(ErrorMessage = "A valid Email address must be entered")]
        [RequiredIf("NoPartyFieldsEntered", true, ErrorMessage = "At least one field is required")]
        public string EmailAddress { get; set; }

        [RequiredIf("NoPartyFieldsEntered", true, ErrorMessage = "At least one field is required")]
        [MaxLength(20, ErrorMessage = "External customer reference cannot be longer than 20 characters.")]
        public string ExternalCustomerReference { get; set; }

        public bool NoPartyFieldsEntered
        {
            get
            {
                return SearchOnParty && string.IsNullOrEmpty(FirstName)
                       && string.IsNullOrEmpty(Surname)
                       && string.IsNullOrEmpty(EmailAddress)
                       && string.IsNullOrEmpty(MobileNumber)
                       && string.IsNullOrEmpty(HomeNumber)
                       && string.IsNullOrEmpty(DateOfBirth)
                       && string.IsNullOrEmpty(ExternalCustomerReference);
            }
        }
    }
}