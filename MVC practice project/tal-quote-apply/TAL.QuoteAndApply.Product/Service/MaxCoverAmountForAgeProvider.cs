using System;
using System.Linq;
using TAL.QuoteAndApply.Product.Definition;

namespace TAL.QuoteAndApply.Product.Service
{
    public interface IMaxCoverAmountForAgeProvider
    {
        int GetFor(string planCode, string brandKey, int age, long annualIncome);
    }

    public class MaxCoverAmountForAgeProvider : IMaxCoverAmountForAgeProvider
    {
        private readonly IPlanDefinitionProvider _planDefinitionProvider;

        public MaxCoverAmountForAgeProvider(IPlanDefinitionProvider planDefinitionProvider)
        {
            _planDefinitionProvider = planDefinitionProvider;
        }

        public int GetFor(string planCode, string brandKey, int age, long annualIncome)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);

            if (planDefinition == null)
            {
                throw new ApplicationException($"No PlanDefinition for plan {planCode}");
            }

            if (planDefinition.AgeRangeCoverAmountDefinitions == null)
            {
                throw new ApplicationException($"No AgeRangeCoverAmountDefinition for plan {planCode}");
            }

            var ageRangeDefinition =
                planDefinition.AgeRangeCoverAmountDefinitions.FirstOrDefault(
                    ageRange => age >= ageRange.AgeRangeDefinition.LowerAge &&
                                age <= ageRange.AgeRangeDefinition.UpperAge);


            if (ageRangeDefinition == null)
            {
                return 0;
            }

            if (!ageRangeDefinition.AnnualIncomeFactor.HasValue)
            {
                return ageRangeDefinition.MaxCoverAmount;
            }

            var annualIncomeMax = (int)(ageRangeDefinition.AnnualIncomeFactor.Value * annualIncome);

            if (ageRangeDefinition.NoIncomeMaxCover.HasValue)
                annualIncomeMax = Math.Max(ageRangeDefinition.NoIncomeMaxCover.Value, annualIncomeMax);

            return Math.Min(annualIncomeMax, ageRangeDefinition.MaxCoverAmount);
        }
    }
}