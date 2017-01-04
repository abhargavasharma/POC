using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators
{
    public class LoadingCalculator
    {
        public static decimal? CalculateLoading(LoadingCalculatorInput input)
        {
            decimal loadingPremium = 0;

            if (input.CoverCalculationRequest.PercentageLoadingSupported)
            {
                loadingPremium += PercentageLoadingCalculator.Calculate(new PercentageLoadingCalculatorInput(input.BasePremium,
                    input.CoverCalculationRequest.Loadings.PercentageLoading, input.PercentageLoadingFactor));
            }

            if (input.CoverCalculationRequest.PerMilleLoadingSupported)
            {
                loadingPremium +=
                    PerMilleLoadingCalculator.Calculate(new PerMilleLoadingCalculatorInput(input.CoverAmount,
                        input.CoverCalculationRequest.Loadings.PerMilleLoading, input.PerMilleLoadingFactor,
                        input.FactorB));
            }

            return loadingPremium;
        }
    }
}
