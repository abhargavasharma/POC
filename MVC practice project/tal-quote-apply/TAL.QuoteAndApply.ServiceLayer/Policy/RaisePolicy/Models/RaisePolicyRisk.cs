using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models
{
    public class RaisePolicyBeneficiary : IBeneficiary
    {
        public string Address { get; set; }
        public string Suburb { get; set; }
        public State State { get; set; }
        public string Postcode { get; set; }
        public Country Country { get; set; }
        public Gender Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int Share { get; set; }
        public int? BeneficiaryRelationshipId { get; set; }
        public Title Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public int Id { get; set; }
        public int RiskId { get; set; }
    }

    public class RaisePolicyRisk : IRisk, IParty
    {
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ResidencyStatus Residency { get; set; }
        public SmokerStatus SmokerStatus { get; set; }
        public string OccupationCode { get; set; }
        public string OccupationTitle { get; set; }
        public string OccupationClass { get; set; }
        public long AnnualIncome { get; set; }
        public bool LprBeneficiary { get; set; }
        public decimal Premium { get; set; }
        public decimal MultiPlanDiscount { get; set; }
        public void AssignOccupationProperties(IOccupationRatingFactors occupationRatingFactors)
        {
            OccupationCode = occupationRatingFactors.OccupationCode;
            OccupationTitle = occupationRatingFactors.OccupationTitle;
            OccupationClass = occupationRatingFactors.OccupationClass;
            IndustryTitle = occupationRatingFactors.IndustryTitle;
            IndustryCode = occupationRatingFactors.IndustryCode;
            IsTpdAny = occupationRatingFactors.IsTpdAny;
            IsTpdOwn = occupationRatingFactors.IsTpdOwn;
            TpdLoading = occupationRatingFactors.TpdLoading;
            PasCode = occupationRatingFactors.PasCode;
        }

        public string InterviewId { get; set; }
        public string InterviewConcurrencyToken { get; set; }
        public string InterviewTemplateVersion { get; set; }
        public InterviewStatus InterviewStatus { get; set; }
        public int PartyId { get; set; }
        public int PolicyId { get; set; }
        public int Id { get; set; }
        public string IndustryTitle { get; set; }
        public string IndustryCode { get; set; }
        public bool IsTpdAny { get; set; }
        public bool IsTpdOwn { get; set; }
        public decimal? TpdLoading { get; set; }
        public string PasCode { get; set; }

        public Title Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public string Address { get; set; }
        public string Suburb { get; set; }
        public State State { get; set; }
        public string Postcode { get; set; }
        public Country Country { get; set; }

        public string MobileNumber { get; set; }
        public string HomeNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ExternalCustomerReference { get; set; }
        public List<RaisePolicyPlan> Plans { get; set; }
        public List<RaisePolicyBeneficiary> Beneficiaries { get; set; }

        public long? LeadId { get; set; }

        public RaisePolicyRisk()
        {
            Beneficiaries = new List<RaisePolicyBeneficiary>();
            Plans = new List<RaisePolicyPlan>();
        }
        
    }

    public class RaisePolicyOwner 
    {
        public int PartyId { get; set; }
        public PolicyOwnerType OwnerType { get; set; }

        public string FundName { get; set; }

        public Title Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public string Address { get; set; }
        public string Suburb { get; set; }
        public State State { get; set; }
        public string Postcode { get; set; }
        public Country Country { get; set; }

        public string MobileNumber { get; set; }
        public string HomeNumber { get; set; }
        public string EmailAddress { get; set; }

        public string ExternalCustomerReference { get; set; }
    }
}