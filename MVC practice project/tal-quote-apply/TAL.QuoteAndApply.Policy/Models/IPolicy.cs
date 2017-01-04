using System;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IPolicy
    {
        int Id { get; }
        string QuoteReference { get; }
        DateTime ModifiedTS { get; }
        string ModifiedBy { get; }
		decimal Premium { get; set; }
        PolicyStatus Status { get; }
        DateTime? SubmittedToRaiseTS { get; }
        bool DeclarationAgree { get; }
        PremiumFrequency PremiumFrequency { get; }
        PolicySaveStatus SaveStatus { get; }
        PolicyProgress Progress { get; }
        PolicySource Source { get; }

        string BrandKey { get; }
        int OrganisationId { get; }
    }
}