using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.Product.Definition
{
    public interface IPlanDefaultsBuilder
    {
        IEnumerable<PlanDefaults> BuildPlanDefaults();
    }

    public class PlanDefaultsBuilder : IPlanDefaultsBuilder
    {
        public IEnumerable<PlanDefaults> BuildPlanDefaults()
        {
            return new List<PlanDefaults>
            {
                new PlanDefaults("DTH", PremiumType.Stepped, true, MarketingStatus.Lead),
                new PlanDefaults("TPDDTH", PremiumType.Stepped, false, MarketingStatus.Lead),
                new PlanDefaults("TRADTH", PremiumType.Stepped, false, MarketingStatus.Lead),
                new PlanDefaults("TRS", PremiumType.Stepped, false, MarketingStatus.Off),
                new PlanDefaults("TPS", PremiumType.Stepped, false, MarketingStatus.Off),
                new PlanDefaults("IP", PremiumType.Stepped, false, 13, 1, MarketingStatus.Lead),
            };
        }
    }
}
