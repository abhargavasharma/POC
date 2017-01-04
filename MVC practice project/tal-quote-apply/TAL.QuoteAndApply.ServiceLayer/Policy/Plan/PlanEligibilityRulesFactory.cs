using System;
using TAL.QuoteAndApply.Policy.Rules.Risk;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Rules.Common;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanEligibilityRulesFactory
    {
        IRule<DateTime> GetMaximumAgeRule(int maximumAge);
        IRule<DateTime> GetMinmumAgeRule(int minimumAge);
        IRule<int> GetMaxCoverAmountMustBeOverMinCoverAmountRule(PlanDefinition planDefinition);
    }

    public class PlanEligibilityRulesFactory : IPlanEligibilityRulesFactory
    {
        public IRule<DateTime> GetMinmumAgeRule(int minimumAge)
        {
            return new MustBeOverMinumumAgeRule(minimumAge, "MinimumAge");
        }

        public IRule<DateTime> GetMaximumAgeRule(int maximumAge)
        {
            return new MustBeUnderMaxumumAgeRule(maximumAge, "MaximumAge");
        }

        public IRule<int> GetMaxCoverAmountMustBeOverMinCoverAmountRule(PlanDefinition planDefinition)
        {
            return new MaxCoverAmountMustBeOverMinCoverAmount(planDefinition);
        }
    }
}