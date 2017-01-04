using System;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models;

namespace TAL.QuoteAndApply.Product.Service
{
    public interface ICoverAmountService
    {
        int GetMaxCover(IMaxCoverAmountParam maxCoverAmountParam);
        int GetMinCover(string planCode, string brandKey);
    }

    public class CoverAmountService : ICoverAmountService
    {
        private readonly IPlanDefinitionProvider _planDefinitionProvider;
        private readonly IMaxCoverAmountForAgeProvider _maxCoverAmountForAgeProvider;
        private readonly IMaxCoverAmountPercentageOfIncomeProvider _maxCoverAmountPercentageOfIncomeProvider;

        public CoverAmountService(IPlanDefinitionProvider planDefinitionProvider, 
            IMaxCoverAmountForAgeProvider maxCoverAmountForAgeProvider,
            IMaxCoverAmountPercentageOfIncomeProvider maxCoverAmountPercentageOfIncomeProvider)
        {
            _planDefinitionProvider = planDefinitionProvider;
            _maxCoverAmountForAgeProvider = maxCoverAmountForAgeProvider;
            _maxCoverAmountPercentageOfIncomeProvider = maxCoverAmountPercentageOfIncomeProvider;
        }

        public int GetMaxCover(IMaxCoverAmountParam maxCoverAmountParam)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(maxCoverAmountParam.PlanCode, maxCoverAmountParam.BrandKey);

            var maxCoverAmount = 0;

            if (planDefinition.AgeRangeCoverAmountDefinitions != null)
            {
                maxCoverAmount = _maxCoverAmountForAgeProvider.GetFor(maxCoverAmountParam.PlanCode, maxCoverAmountParam.BrandKey,
                    maxCoverAmountParam.Age, maxCoverAmountParam.AnnualIncome);
            }
            else if (planDefinition.CoverAmountPercentageDefinition != null)
            {
                maxCoverAmount = _maxCoverAmountPercentageOfIncomeProvider.GetFor(maxCoverAmountParam.PlanCode, maxCoverAmountParam.BrandKey,
                    maxCoverAmountParam.AnnualIncome);
            }
            else
            {
                throw new ApplicationException($"No max cover amount definition provided for plan {maxCoverAmountParam.PlanCode}");
            }

            var parentCapCoverAmountValue = maxCoverAmountParam.ParentPlanCoverCap.GetValueOrDefault(int.MaxValue);
            if (parentCapCoverAmountValue <= 0)
            {
                parentCapCoverAmountValue = int.MaxValue;
            }

            return Math.Min(maxCoverAmount, parentCapCoverAmountValue);
        }

        public int GetMinCover(string planCode, string brandKey)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);
            return planDefinition.MinimumCoverAmount;
        }
    }
}