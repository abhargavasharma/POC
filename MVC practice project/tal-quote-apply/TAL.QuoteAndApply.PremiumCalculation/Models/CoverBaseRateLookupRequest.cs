using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class CoverBaseRateLookupRequest : ICoverBaseRateCriteria
    {
        public string CoverCode { get; }
        public int BrandId { get; }
        public string PlanCode { get; }
        public int Age { get; }
        public Gender Gender { get; }
        public PremiumType PremiumType { get; }
        public string OccupationGroup { get; }
        public int? BenefitPeriod { get; }
        public int? WaitingPeriod { get; }
        public bool? IsSmoker { get; }
        public bool? BuyBack { get; }

        public CoverBaseRateLookupRequest(string planCode, string coverCode, int age, Gender gender, PremiumType premiumType, bool? isSmoker, int? benefitPeriod, int? waitingPerioid, string occupationGroup, bool? buyBack, int brandId)

        {
            PlanCode = planCode;
            CoverCode = coverCode;
            Age = age;
            Gender = gender;
            PremiumType = premiumType;
            IsSmoker = isSmoker;
            BenefitPeriod = benefitPeriod;
            WaitingPeriod = waitingPerioid;
            OccupationGroup = occupationGroup;
            BuyBack = buyBack;
            BrandId = brandId;
        }

        public override string ToString()
        {
            return $"PlanCode: {PlanCode}; CoverCode: {CoverCode}; Age {Age}; Gender: {Gender}; PremiumType: {PremiumType}; " +
                   $"IsSmoker: {IsSmoker}; OccupationGroup: {OccupationGroup}; BenefitPeriod: {BenefitPeriod}; " +
                   $"WaitingPeriod: {WaitingPeriod}; BuyBack: {BuyBack}; Brand: {BrandId}";
        }
    }
}