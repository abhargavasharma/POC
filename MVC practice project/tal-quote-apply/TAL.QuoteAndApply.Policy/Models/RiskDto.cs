using System;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class RiskDto : DbItem, IRisk, IRatingFactors, IUnderwritingClient, IEnterprisePartyClient
    {
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ResidencyStatus Residency { get; set; }
        public SmokerStatus SmokerStatus { get; set; }
        public long AnnualIncome { get; set; }
        public bool LprBeneficiary { get; set; }
        public decimal Premium { get; set; }
        public decimal MultiPlanDiscount { get; set; }
        public string InterviewId { get; set; }
        public string InterviewConcurrencyToken { get; set; }
        public string InterviewTemplateVersion { get; set; }
        public InterviewStatus InterviewStatus { get; set; }
        public int PartyId { get; set; }
        public int PolicyId { get; set; }

        public string OccupationCode { get; set; }
        public string OccupationTitle { get; set; }
        public string OccupationClass { get; set; }
        public string IndustryTitle { get; set; }
        public string IndustryCode { get; set; }
        public bool IsTpdAny { get; set; }
        public bool IsTpdOwn { get; set; }
        public decimal? TpdLoading { get; set; }
        public string PasCode { get; set; }

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
    }
}