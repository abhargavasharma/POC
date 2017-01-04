using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators
{
    public class PerMilleLoadingCalculator
    {
        private const decimal DivisionalFactor = 1000;
        private const decimal DivisionalFactorModal = 11;

        public static decimal Calculate(PerMilleLoadingCalculatorInput input)
        {
            var loadingFactor = input.PerMilleLoading + input.PerMilleLoadingFactor;

            var loading = loadingFactor * input.CoverAmount;

            loading = loading / DivisionalFactor;
            loading = loading / DivisionalFactorModal;

            loading = loading * input.FactorB;

            return loading.RoundToTwoDecimalPlaces();
        }
    }
}
