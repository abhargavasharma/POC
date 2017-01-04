using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models
{
    public class RaisePolicy : IPolicy
    {
        public int Id { get; set; }
        public string QuoteReference { get; set; }
		public string BrandKey { get; set; }
        public int OrganisationId { get; set; }
        
        public decimal Premium { get; set; }
        public RaisePolicyPayment Payment { get; set; }
        public PremiumFrequency PremiumFrequency { get; set; }
        public PolicySaveStatus SaveStatus { get; set; }
        public PolicyProgress Progress { get; set; }
        public PolicySource Source { get; set; }
        public PolicyStatus Status { get; set; }
        public bool DeclarationAgree { get; set; }
        
        public List<RaisePolicyRisk> Risks { get; set; }
        public RaisePolicyRisk PrimaryRisk { get; set; }
		public RaisePolicyOwner Owner { get; set; }

        public DateTime ReadyToSubmitDateTime { get; set; }
        public DateTime? LastCompletedReferralDateTime { get; set; }
        public DateTime? SubmittedToRaiseTS { get; set; }
        public DateTime ModifiedTS { get; set; }
        public string ModifiedBy { get; set; }

        public string DocumentUrl { get; set; }

        public bool HasLoadings
        {
            get
            {
                if (PrimaryRisk != null && PrimaryRisk.Plans != null)
                {
                    var allLoadings = PrimaryRisk.Plans.SelectMany(p => p.Covers).SelectMany(c => c.Loadings);
                    return allLoadings.Any();
                }

                return false;
            }
        }

        public bool HasExclusions
        {
            get
            {
                if (PrimaryRisk != null && PrimaryRisk.Plans != null)
                {
                    var allExclusions = PrimaryRisk.Plans.SelectMany(p => p.Covers).SelectMany(c => c.Exclusions);
                    return allExclusions.Any();
                }

                return false;
            }
        }
    }
}
