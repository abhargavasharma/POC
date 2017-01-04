using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Services
{
    public interface IGetMultiCoverBlockCalculatorInputService
    {
        MultiCoverBlockCalculatorInput GetMultiCoverBlockCalculatorInput(IPlanFactors planFactors, IReadOnlyList<ICoverFactors> coverFactors, IReadOnlyList<CoverPremiumCalculationResult> coverPremiumResults, int brandId);
    }

    public class GetMultiCoverBlockCalculatorInputService : IGetMultiCoverBlockCalculatorInputService
    {
        private readonly IGetMultiCoverDiscountCalculatorInputService _getMultiCoverDiscountCalculatorInputService;

        public GetMultiCoverBlockCalculatorInputService(IGetMultiCoverDiscountCalculatorInputService getMultiCoverDiscountCalculatorInputService)
        {
            _getMultiCoverDiscountCalculatorInputService = getMultiCoverDiscountCalculatorInputService;
        }

        public MultiCoverBlockCalculatorInput GetMultiCoverBlockCalculatorInput(IPlanFactors planFactors, IReadOnlyList<ICoverFactors> coverFactors, IReadOnlyList<CoverPremiumCalculationResult> coverPremiumResults, int brandId)
        {
            var activeCoversIncludedInMultiCover = coverFactors.Where(c => c.Active && c.IsRateableCover).ToList();
            var planDiscountInput = _getMultiCoverDiscountCalculatorInputService.GetPlanDiscountCalculatorInput(planFactors, activeCoversIncludedInMultiCover, coverPremiumResults, brandId);

            var activeCoverCalcResults = coverPremiumResults.Where(cr => activeCoversIncludedInMultiCover.Select(c => c.CoverCode).Contains(cr.CoverCode)).ToList();

            return new MultiCoverBlockCalculatorInput(planDiscountInput, activeCoverCalcResults.Select(c=> c.BasePremium).ToList());
        }
    }
}
