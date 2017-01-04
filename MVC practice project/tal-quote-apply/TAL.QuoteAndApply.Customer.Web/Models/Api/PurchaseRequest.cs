using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class PurchaseRequest
    {
        public bool IsComplete { get; set; }
        public bool NominateLpr { get; set; }
        [BoolIsTrue(ErrorMessage = "You need to declare you have read and agree with the terms above.")]
        [Required(ErrorMessage = "You need to declare you have read and agree with the terms above.")]
        public bool DeclarationAgree { get; set; }
        public bool DncSelection { get; set; }
        public int RiskId { get; set; }

        public PersonalDetailsViewModel PersonalDetails { get; set; }

        public PolicyNoteResultViewModel DisclosureNotes { get; set; }

        public List<BeneficiaryViewModel> Beneficiaries { get; set; }

        public PaymentOptionsViewModel PaymentOptions { get; set; }
    }
}