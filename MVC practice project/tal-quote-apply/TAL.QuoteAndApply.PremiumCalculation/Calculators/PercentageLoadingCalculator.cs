using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators
{
    public class PercentageLoadingCalculator
    {
        public static decimal Calculate(PercentageLoadingCalculatorInput percentageLoadingCalculatorInput)
        {
            var loadingFactor = (percentageLoadingCalculatorInput.PercentageLoading / 100) + percentageLoadingCalculatorInput.PercentageLoadingFactor;

            var loading = percentageLoadingCalculatorInput.Premium * loadingFactor;

            return loading.RoundToTwoDecimalPlaces();
        }
    }
}
