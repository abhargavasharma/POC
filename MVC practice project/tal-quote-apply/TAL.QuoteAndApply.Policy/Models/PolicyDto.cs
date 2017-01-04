using System;
using System.Runtime.CompilerServices;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;

[assembly: InternalsVisibleTo("TAL.QuoteAndApply.ServiceLayer.UnitTests")]

namespace TAL.QuoteAndApply.Policy.Models
{
    public class PolicyDto : DbItem, IPolicy
    {
        public string QuoteReference { get; set; }
        public PolicyStatus Status { get; set; }
        public DateTime? SubmittedToRaiseTS { get; set; }
        public decimal Premium { get; set; }
        public PremiumFrequency PremiumFrequency { get; set; }
        public bool DeclarationAgree { get; set; }
        public PolicySaveStatus SaveStatus { get; set; }
        public PolicyProgress Progress { get; set; }
        public PolicySource Source { get; set; }
        public int BrandId { get; set; }
        public string BrandKey { get; internal set; }
        public int OrganisationId { get; set; }
    }
}
