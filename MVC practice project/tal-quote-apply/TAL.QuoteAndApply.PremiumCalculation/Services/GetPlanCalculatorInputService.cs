using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Services
{
    public interface IGetPlanCalculatorInputService
    {
        PlanCalculatorInput GetPlanCalculatorInput(IPlanFactors planFactors, IEnumerable<IPlanFactors> allPlans, IReadOnlyList<ICoverFactors> coverFactors,
            IReadOnlyList<CoverPremiumCalculationResult> coverPremiumResults, int brandId);
    }

    public class GetPlanCalculatorInputService : IGetPlanCalculatorInputService
    {
        private readonly IGetMultiCoverDiscountCalculatorInputService _getMultiCoverDiscountCalculatorInputService;
        private readonly IMultiPlanDiscountFactorService _multiPlanDiscountFactorService;

        public GetPlanCalculatorInputService(IGetMultiCoverDiscountCalculatorInputService getMultiCoverDiscountCalculatorInputService, IMultiPlanDiscountFactorService multiPlanDiscountFactorService)
        {
            _getMultiCoverDiscountCalculatorInputService = getMultiCoverDiscountCalculatorInputService;
            _multiPlanDiscountFactorService = multiPlanDiscountFactorService;
        }

        public PlanCalculatorInput GetPlanCalculatorInput(IPlanFactors planFactors, IEnumerable<IPlanFactors> allPlans, IReadOnlyList<ICoverFactors> coverFactors, IReadOnlyList<CoverPremiumCalculationResult> coverPremiumResults, int brandId)
        {
            var activeCovers = coverFactors.Where(c => c.Active).ToList();
            var planDiscountInput = _getMultiCoverDiscountCalculatorInputService.GetPlanDiscountCalculatorInput(planFactors, activeCovers, coverPremiumResults, brandId);

            var activeCoverCalcResults = coverPremiumResults.Where(cr => activeCovers.Select(c => c.CoverCode).Contains(cr.CoverCode)).ToList();

            var multiPlanDiscountFactor = _multiPlanDiscountFactorService.GetFor(planFactors, allPlans, brandId);

            return new PlanCalculatorInput(planDiscountInput, activeCoverCalcResults, multiPlanDiscountFactor);
        }
    }
}