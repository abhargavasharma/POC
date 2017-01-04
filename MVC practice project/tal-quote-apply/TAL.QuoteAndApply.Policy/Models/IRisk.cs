using System;
using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IRisk : IOccupationRatingFactors
    {
        int PolicyId { get; set; }
		string InterviewId { get; set; }		
		string InterviewConcurrencyToken { get; set; }	
        string InterviewTemplateVersion { get; set; }
        InterviewStatus InterviewStatus { get; set; }
        int PartyId { get; set; }
        Gender Gender { get; set; }
        DateTime DateOfBirth { get; set; }
        ResidencyStatus Residency { get; set; }
        SmokerStatus SmokerStatus { get; set; }
        long AnnualIncome { get; set; }
        bool LprBeneficiary { get; set; }
        decimal Premium { get; set; }
        decimal MultiPlanDiscount { get; set; }

        void AssignOccupationProperties(IOccupationRatingFactors occupationRatingFactors);
    }
}