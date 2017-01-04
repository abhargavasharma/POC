using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators
{
    public class MultiCoverDiscountCalculator
    {
        public static decimal CalculateDiscount(MultiCoverDiscountCalculatorInput input)
        {
            var includedCovers = input.Covers.Where(c => c.IncludeInMultiCoverDiscount).ToList();

            if (includedCovers.Count > 1)
            {
                var includedPremium = includedCovers.Sum(c => c.Premium);
                return (includedPremium * input.MultiCoverDiscountFactor).RoundToTwoDecimalPlaces();
            }

            return 0m;
        }
    }
}
