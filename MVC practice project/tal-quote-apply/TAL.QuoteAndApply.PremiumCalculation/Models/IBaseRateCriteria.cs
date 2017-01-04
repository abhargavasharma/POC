using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public interface ICoverBaseRateCriteria
    {
        string CoverCode { get; }
        int BrandId { get; }
        string PlanCode { get; }
        int Age { get; }
        Gender Gender { get; }
        PremiumType PremiumType { get; }
        string OccupationGroup { get; }
        int? BenefitPeriod { get; }
        int? WaitingPeriod { get; }
        bool? IsSmoker { get; }
        bool? BuyBack { get; }
    }

}