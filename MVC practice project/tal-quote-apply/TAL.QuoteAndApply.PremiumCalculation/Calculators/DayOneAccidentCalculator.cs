using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators
{
    public class DayOneAccidentCalculator
    {
        public static decimal Calculate(DayOneAccidentCalculatorInput input)
        {
            if (!input.DayOneAccidentSelected.GetValueOrDefault(false))
            {
                return 0;
            }

            //start with base rate
            var premium = input.BaseRate;

            //multiply by occupation loading
            premium = premium*input.OccupationFactor;

            //multiply by cover amount
            premium = premium*input.CoverAmount;

            //divide by 1000
            premium = premium/1000;

            //multiply by factor B
            premium = premium*input.FactorB;

            return premium.RoundToTwoDecimalPlaces();
        }
    }
}
