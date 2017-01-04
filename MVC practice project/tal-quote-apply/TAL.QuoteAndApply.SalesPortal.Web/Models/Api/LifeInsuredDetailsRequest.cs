using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class LifeInsuredDetailsRequest
    {
        public string Title { get; set; }
        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        [MaxLength(22, ErrorMessage = "First name cannot be longer than 22 characters.")]
        public string FirstName { get; set; }
        [NameValidation(ErrorMessage = "Name must not contain numbers")]
        [MaxLength(22, ErrorMessage = "Surname cannot be longer than 22 characters.")]
        public string Surname { get; set; }

        [NumbersOnlyValidation(ErrorMessage = "Postcode must not contain any non numeric values")]
        [StringLengthAllowNullsAndEmpty(MinLength = 4, MaxLength = 4, ErrorMessage = "Postcode must be 4 digits")]
        public string Postcode { get; set; }

        public bool ExpressConsent { get; set; }
    }
}