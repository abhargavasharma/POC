using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators
{
    public class CoverCalculator
    {
        public static decimal Calculate(CoverCalculatorInput input)
        {
            //base rate
            var premium = input.BaseRate;

            //multiply by factor A
            premium = premium*input.FactorA;

            //multiply by cover ammount
            premium = premium*input.CoverAmount;

            //divide by divisional factor
            premium = premium/input.DivionalFactor;

            //multiply by factor B
            premium = premium*input.FactorB;

            //round to 2 decimal places
            return premium.RoundToTwoDecimalPlaces();
        }
    }
}
