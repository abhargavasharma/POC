using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PolicyOverviewResult
    {
        public int PolicyId { get; set; }
        public string QuoteReferenceNumber { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public string LastModifiedBy { get; set; }
        public PolicyStatus Status { get; set; }
        public IList<RiskOverviewResult> Risks { get; set; }
        public int? OwnerRiskId { get; set; }
        public bool DeclarationAgree { get; set; }
        public decimal Premium { get; set; }
        public PremiumFrequency PremiumFrequency { get; set; }
        public PolicySource Source { get; set; }
        public string Brand { get; set; }
		public PolicyOwnerType OwnerType { get; set; }
    }

    public class RiskOverviewResult
    {
        public int RiskId { get; set; }
        public string InterviewConcurrencyToken { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public long AnnualIncome { get; set; }
        public InterviewStatus InterviewStatus { get; set; }
        public List<OccupationDefinition> AvailableDefinitions { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string ExternalCustomerReference { get; set; }
    }
}
