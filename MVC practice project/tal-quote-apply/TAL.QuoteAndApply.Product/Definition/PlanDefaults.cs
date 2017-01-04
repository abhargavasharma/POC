using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.Product.Definition
{
    public class PlanDefaults
    {
        public string Code { get; }
        public PremiumType PremiumType { get; }
        public bool Selected { get; }
        public int? WaitingPeriod { get; }
        public int? BenefitPeriod { get; }
        public OccupationDefinition OccupationDefinition { get; }
        public MarketingStatus MarketingStatus { get; }

        public PlanDefaults(string code, PremiumType premiumType, bool selected, MarketingStatus marketingStatus)
        {
            Code = code;
            PremiumType = premiumType;
            Selected = selected;
            MarketingStatus = marketingStatus;
        }

        public PlanDefaults(string code, PremiumType premiumType, bool selected, int waitingPeriod, int benefitPeriod, MarketingStatus marketingStatus)
        {
            Code = code;
            PremiumType = premiumType;
            Selected = selected;
            WaitingPeriod = waitingPeriod;
            BenefitPeriod = benefitPeriod;
            MarketingStatus = marketingStatus;
        }
    }
}