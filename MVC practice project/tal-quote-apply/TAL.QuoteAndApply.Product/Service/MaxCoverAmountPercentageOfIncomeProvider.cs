using System;
using TAL.QuoteAndApply.Product.Definition;

namespace TAL.QuoteAndApply.Product.Service
{
    public interface IMaxCoverAmountPercentageOfIncomeProvider
    {
        int GetFor(string planCode, string brandKey, long annualIncome);
    }

    public class MaxCoverAmountPercentageOfIncomeProvider : IMaxCoverAmountPercentageOfIncomeProvider
    {
        private readonly IPlanDefinitionProvider _planDefinitionProvider;

        public MaxCoverAmountPercentageOfIncomeProvider(IPlanDefinitionProvider planDefinitionProvider)
        {
            _planDefinitionProvider = planDefinitionProvider;
        }

        public int GetFor(string planCode, string brandKey, long annualIncome)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);

            if (planDefinition == null)
            {
                throw new ApplicationException($"No PlanDefinition for plan {planCode}");
            }

            if (planDefinition.CoverAmountPercentageDefinition == null)
            {
                throw new ApplicationException($"No CoverAmountPercentageDefinition for plan {planCode}");
            }

            var percentageOfIncome = Convert.ToInt32( (annualIncome/12) *(planDefinition.CoverAmountPercentageDefinition.PercentageOfIncome/100m));

            return Math.Min(percentageOfIncome, planDefinition.CoverAmountPercentageDefinition.MaxCoverAmount);
        }
    }
}