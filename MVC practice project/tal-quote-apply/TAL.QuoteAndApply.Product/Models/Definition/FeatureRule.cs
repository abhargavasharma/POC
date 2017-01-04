using System.Collections.Generic;

namespace TAL.QuoteAndApply.Product.Models.Definition
{
    public class FeatureRule
    {
        public FeatureRule(IEnumerable<ProductConfigRule> planConfigRules, IEnumerable<ProductConfigRule> coverConfigRules)
        {
            PlanConfigRules = planConfigRules;
            CoverConfigRules = coverConfigRules;
        }

        public IEnumerable<ProductConfigRule> PlanConfigRules { get; private set; }
        public IEnumerable<ProductConfigRule> CoverConfigRules { get; private set; }
    }
}