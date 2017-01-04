using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Services
{
    public interface IGetMultiCoverDiscountCalculatorInputService
    {
        MultiCoverDiscountCalculatorInput GetPlanDiscountCalculatorInput(IPlanFactors planFactors, IReadOnlyList<ICoverFactors> activeCoverFactors, IReadOnlyList<CoverPremiumCalculationResult> coverPremiumResults, int brandId);
    }

    public class GetMultiCoverDiscountCalculatorInputService : IGetMultiCoverDiscountCalculatorInputService
    {
        private readonly IMultiCoverDiscountFactorDtoRepository _multiCoverDiscountFactorDtoRepository;

        public GetMultiCoverDiscountCalculatorInputService(IMultiCoverDiscountFactorDtoRepository multiCoverDiscountFactorDtoRepository)
        {
            _multiCoverDiscountFactorDtoRepository = multiCoverDiscountFactorDtoRepository;
        }

        public MultiCoverDiscountCalculatorInput GetPlanDiscountCalculatorInput(IPlanFactors planFactors, 
            IReadOnlyList<ICoverFactors> activeCoverFactors, 
            IReadOnlyList<CoverPremiumCalculationResult> coverPremiumResults, int brandId)
        {

            var multiCoverDiscountFactor = GetMultiCoverDiscountFactor(planFactors.PlanCode, brandId, activeCoverFactors);

            var coverInputs = GetPlanDiscountCoverInputs(activeCoverFactors, coverPremiumResults);

            return new MultiCoverDiscountCalculatorInput(multiCoverDiscountFactor, coverInputs);
        }

        private static List<MultiCoverDiscountCoverCalculatorInput> GetPlanDiscountCoverInputs(IReadOnlyList<ICoverFactors> activeCoverFactors, 
            IReadOnlyList<CoverPremiumCalculationResult> coverPremiumResults)
        {
            var coverInputs = new List<MultiCoverDiscountCoverCalculatorInput>();

            foreach (var ac in activeCoverFactors)
            {
                var coverPremResult = coverPremiumResults.FirstOrDefault(c => c.CoverCode == ac.CoverCode);

                if (coverPremResult == null)
                    continue;

                coverInputs.Add(new MultiCoverDiscountCoverCalculatorInput(coverPremResult.BasePremium,
                    ac.IsRateableCover));
            }
            return coverInputs;
        }

        private decimal GetMultiCoverDiscountFactor(string planCode, int brandId, IReadOnlyList<ICoverFactors> activeCoverFactors)
        {
            var activeCoverCodes = activeCoverFactors.Where(c=> c.IsRateableCover).Select(c => c.CoverCode).OrderBy(c=> c);

            var lookupResult = _multiCoverDiscountFactorDtoRepository.GetMultiCoverDiscountFactorForPlan(planCode, brandId, string.Join("|", activeCoverCodes));

            if (lookupResult == null)
            {
                return 0m;
            }

            return lookupResult.Factor;
        }
    }
}
