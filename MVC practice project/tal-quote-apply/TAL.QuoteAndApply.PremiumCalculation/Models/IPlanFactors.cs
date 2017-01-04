using System.Security.Cryptography.X509Certificates;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public interface IPlanFactors
    {
        string PlanCode { get; }
        decimal CoverAmount { get; }
        int? WaitingPeriod { get; }
        int? BenefitPeriod { get; }
        bool? BuyBack { get; }
        PremiumType PremiumType { get; }
        bool IncludeInMultiPlanDiscount { get; }
        bool? PremiumReliefOptionSelected { get; }
        bool? IncreasingClaimsSelected { get; }
        bool? DayOneAccidentSelected { get; }
        bool Active { get; }
        OccupationDefinition OccupationDefinition { get; }
        decimal? OccupationLoading { get; }
    }
}