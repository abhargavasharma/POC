using System;
using System.Collections.Generic;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class RetrievePolicyViewModel
    {
        public int PolicyId { get; set; }
        public string QuoteReferenceNumber { get; set; }
        public DateTime LastSavedDate { get; set; }
        public IEnumerable<RetrievePolicyRiskViewModel> Risks { get; set; }
        public ViewModelPolicyStatus Status { get; set; }
        public bool ReadOnly { get; set; }
        public string UserRole { get; set; }
        public RetrievePolicyViewModel()
        {
            Risks = new List<RetrievePolicyRiskViewModel>();
        }

        public QuoteEditSource QuoteEditSource { get; set; }
        public string OwnerType { get; set; }
    }

    public class RetrievePolicyRiskViewModel
    {
        public int RiskId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
    }
}